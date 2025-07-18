using System.Collections;
using System.Collections.Generic;
using Singletons;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;
using UnityEngine.UI;

public class StartVideo : MonoBehaviour
{ 
    [SerializeField] private VideoPlayer videoPlayer; 
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;
    
    private bool _skipped;

    void Start()
    {
        if (GameManager.Instance.StartVideoHasBeenPlayed())
        {
            gameObject.SetActive(false);
            return;
        }
        
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_skipped)
        {
            SkipVideo();
        }
    }

    private void SkipVideo()
    {
        _skipped = true;
        videoPlayer.Stop();
        gameObject.SetActive(false);
        GameManager.Instance.FinishStartVideo();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!_skipped)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}