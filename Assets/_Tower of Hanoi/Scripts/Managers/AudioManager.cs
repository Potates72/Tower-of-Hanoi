using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource, sfxSource;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void ToggleBGM()
    {
        bgmSource.mute = !bgmSource.mute;
    }
}
