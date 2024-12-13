using UnityEngine;
using System.Collections;

namespace SpaceGame.Extensions
{
    public static class AudioSourceExtension
    {
        private static IEnumerator FadeOutCoroutine(this AudioSource audioSource, float fadeTime)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }
}
