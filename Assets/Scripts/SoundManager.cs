using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    [SerializeField]
    private AudioSource audioFxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public AudioSource PlaySoundClip(AudioClip audioClip, Transform spawnTransform, float volume = 0.5f, float minPitch = 0.5f, float maxPitch = 1.1f)
    {
        var audioSource = Instantiate(audioFxSource, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
        return audioSource;
    }

    public AudioSource PlayRandomSoundClip(AudioClip[] audioClip, Transform spawnTransform, float volume = 0.5f, float minPitch = 0.5f, float maxPitch = 1.1f)
    {
        var randomIndex = Random.Range(0, audioClip.Length);
        return PlaySoundClip(audioClip[randomIndex], spawnTransform, volume, minPitch, maxPitch);
    }
}
