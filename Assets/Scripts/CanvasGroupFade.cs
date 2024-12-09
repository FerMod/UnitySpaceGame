using System;
using System.Collections;
using UnityEngine;

namespace SpaceGame.UI
{
    public class CanvasGroupFade : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private float fadeDuration = 1.0f;

        public void FadeIn()
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration));
        }

        public void FadeOut()
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, fadeDuration));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = end;
        }
    }
}
