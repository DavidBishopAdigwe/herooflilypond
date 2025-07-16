using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers;
using Enums;

public class MessageMaster : GameObjectSingleton<MessageMaster>
{
    [Header("References")]
    [SerializeField] private Message messagePrefab;
    public Transform messageParent;

    [Header("Pool Settings")]
    public int maxMessages = 3;

    private Queue<Message> _activeQueue = new Queue<Message>();
    private List<Message> _pool = new List<Message>();

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        CreatePool();
    }

    private void CreatePool()
    {
        if (_pool.Count > 0) return;

        for (int i = 0; i < maxMessages; i++)
        {
            var msg = Instantiate(messagePrefab, messageParent);
            msg.gameObject.SetActive(true);
            _pool.Add(msg);
            msg.gameObject.SetActive(false);

        }
    }



    public void ShowMessage(string message, MessageType type = MessageType.Default)
    {
        CreatePool();
        Message msg;
        if (_activeQueue.Count < maxMessages)
        {
            msg = _pool[0];
            _pool.RemoveAt(0);
        }
        else
        {
            msg = _activeQueue.Dequeue();
            msg.StopAllCoroutines();
        }
    
        _activeQueue.Enqueue(msg);
        msg.transform.SetSiblingIndex(_activeQueue.Count - 1);
        msg.Display(message, type);
    }


    public void MessageCompleted(Message msg)
    {
        var newQueue = new Queue<Message>();
        foreach (var m in _activeQueue)
        {
            if (m != msg) newQueue.Enqueue(m);
        }
        _activeQueue = newQueue;

        ReturnToPool(msg);
    }

    private void ReturnToPool(Message msg)
    {
        if (!_pool.Contains(msg))
        {
            _pool.Add(msg);
        }
    }
}