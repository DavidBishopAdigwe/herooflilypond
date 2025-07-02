using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    [CreateAssetMenu(fileName = "Message Manager", menuName = "ScriptableObjects/MessageManager")]
    public class MessageManager : ScriptableObjectSingleton<MessageManager>
    {
        
        [SerializeField] private float messageDuration; 
        [SerializeField] private int maxMessages;
        [SerializeField] private GameObject messagePrefab; 
        // [SerializeField] private Transform messageParent;

        private Queue<GameObject> _messages = new Queue<GameObject>();
        
        public void ShowMessage(string text)
        {
            GameObject newMessage = Instantiate(messagePrefab);
            TMP_Text tmp = newMessage.GetComponent<TMP_Text>();
            tmp.text = text;

            _messages.Enqueue(newMessage);

            if (_messages.Count > maxMessages)
            {
                GameObject oldMessage = _messages.Dequeue();
                Destroy(oldMessage);
            }

            Destroy(newMessage, messageDuration);
        }
    
    }
}