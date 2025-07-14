
using System;
using TMPro;
using UnityEngine;

public class Messages: MonoBehaviour
{

    [SerializeField] private TMP_Text messageObj;
    public static Messages Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ClearMessage();
    }

    public void DisplayMessage(string msg, int duration)
    {
        messageObj.text = msg;
        Invoke("ClearMessage", duration);
    }

    public void ClearMessage()
    {
        messageObj.text = " ";
    }
} 

