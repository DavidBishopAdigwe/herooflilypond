using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ScreenControl : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    private readonly List<Vector2Int> _supportedResolutions = new()
    {
        new (1920, 1080),
        new (1600, 900),
        new (1366, 768),
        new (1280, 720),
    };

    private void OnEnable()
    {
        Screen.fullScreen = false;
        dropdown.ClearOptions();

        List<string> options = new();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _supportedResolutions.Count; i++)
        {
            Vector2Int res = _supportedResolutions[i];
            string option = $"{res.x} x {res.y}";
            options.Add(option);

            if (Screen.currentResolution.width == res.x && Screen.currentResolution.height == res.y)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
        dropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Vector2Int res = _supportedResolutions[index];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    
}