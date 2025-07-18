using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Enums;
using UnityEngine.Serialization;

public class Message : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text text;
    private CanvasGroup _canvasGroup;
    private Coroutine _lifecycle;
    private RectTransform _rectTransform;
    
    public float fadeDuration = 0.25f;
    public float displayDuration = 2f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Display(string message, MessageType type = MessageType.Default)
    {
        InternalDisplay(message, type);
        _lifecycle = StartCoroutine(Lifecycle());
    }

    public void Display(string message, float duration, MessageType type = MessageType.Default)
    {
        InternalDisplay(message, type);
        _lifecycle = StartCoroutine(Lifecycle(duration));
    }

    public void Display(string message, Func<bool> condition, MessageType type)
    {
        InternalDisplay(message, type);
        _lifecycle = StartCoroutine(Lifecycle(condition));
    }

    private void InternalDisplay(string message, MessageType type = MessageType.Default)
    {
        if (_lifecycle != null)
        {
            StopCoroutine(_lifecycle);
        }
        
        text.text = message;
        background.color = MessageColour(type);
        var pos = _rectTransform.position;
        pos.x = 0;
        _rectTransform.position = pos;
        gameObject.SetActive(true);
    }

    private IEnumerator Lifecycle()
    {
        yield return Fade(0f, 1f, fadeDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1f, 0f, fadeDuration);
        
        FinishMessage();
    }

    private IEnumerator Lifecycle(Func<bool> condition)
    {
        yield return Fade(0f, 1f, fadeDuration);
        yield return new WaitUntil(condition);
        yield return Fade(1f, 0f, fadeDuration);
        
        FinishMessage();
    }
    
    private IEnumerator Lifecycle(float duration)
    {
        yield return Fade(0f, 1f, duration);
        yield return new WaitForSeconds(duration);
        yield return Fade(1f, 0f, duration);
        gameObject.SetActive(false);
        MessageManager.Instance.MessageCompleted(this);
        
    }

    public void EndMessageAbruptly()
    {
        if (!gameObject.activeInHierarchy) return; 
        
        if (_lifecycle != null)
        {
            StopCoroutine(_lifecycle);
        }
        var fadeRoutine = StartCoroutine(Fade(1f, 0f, fadeDuration));
        FinishMessage();
    }

    private void FinishMessage()
    {
        gameObject.SetActive(false);
        MessageManager.Instance.MessageCompleted(this);
    }


    
    

    private IEnumerator Fade(float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            _canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _canvasGroup.alpha = end;
    }

    private Color MessageColour(MessageType t)
    {
        switch (t)
        {
            case MessageType.Warning: return new Color(1f, 0.8f, 0.2f, 0.14f);
            case MessageType.Error:   return new Color(1f, 0.3f, 0.3f, 0.14f);
            case MessageType.Success: return new Color(0.3f, 1f, 0.4f, 0.14f);
            case MessageType.Default:    
            default:                   return new Color(0.6f, 0.8f, 1f, 0.14f);
        }
    }
}