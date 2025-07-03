using System;
using System.IO;
using DataPersistence.Data;
using UnityEngine;


public class FileDataHandler
{
    private string _filePath;

    private string _fileName;

    public FileDataHandler(string filePath, string fileName)
    {
        _filePath = filePath;
        _fileName = fileName;
    }
    
    public GameData Load()
    {
        string fullPath = Path.Combine(_filePath, _fileName);
        
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using Stream stream = new FileStream(fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                dataToLoad = reader.ReadToEnd();
                
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading file: " + fullPath + "/n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(_filePath, _fileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException());
            
            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + fullPath + "/n" + e);
        }
    }
}
