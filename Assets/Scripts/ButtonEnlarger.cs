
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UIElements;
    using Cursor = UnityEngine.Cursor;

    public class ButtonEnlarger: MonoBehaviour
    {
        
     [SerializeField] private float minSize; 
     [SerializeField] private float maxSize; 
     private TMP_Text _text;

     
       private void Start()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        public void Enlarge()
        {
            _text.fontSize = Mathf.Max(minSize, maxSize);
        }

        public void Shrink()
        {
            _text.fontSize = Mathf.Min(minSize, maxSize);
        }
        
        
    }
