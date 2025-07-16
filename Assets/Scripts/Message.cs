using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Enums;
using UnityEngine.Serialization;

public class Message : MonoBehaviour
{
    [FormerlySerializedAs("_background")] [SerializeField] private Image background;
    [FormerlySerializedAs("_text")] [SerializeField] private TMP_Text text;
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

    public void Display(string message, MessageType type)
    {
        if (_lifecycle != null)
        {
            StopCoroutine(_lifecycle);
        }
        
        text.text = message;
        background.color = Colours(type);
        var pos = _rectTransform.position;
        pos.x = 0;
        _rectTransform.position = pos;
        gameObject.SetActive(true);
        _lifecycle = StartCoroutine(Lifecycle());
    }

    private IEnumerator Lifecycle()
    {
        yield return Fade(0f, 1f, fadeDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1f, 0f, fadeDuration);

        gameObject.SetActive(false);
        MessageMaster.Instance.MessageCompleted(this);
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

    private Color Colours(MessageType t)
    {
        switch (t)
        {
            case MessageType.Warning: return new Color(1f, 0.8f, 0.2f, (35/255f));
            case MessageType.Error:   return new Color(1f, 0.3f, 0.3f, (35/255f));
            case MessageType.Success: return new Color(0.3f, 1f, 0.4f, (35/255f));
            case MessageType.Default:    
            default:                   return new Color(0.6f, 0.8f, 1f, (35/255f));
        }
    }
}