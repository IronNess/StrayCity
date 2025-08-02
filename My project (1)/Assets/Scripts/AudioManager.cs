using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip catMeow;
    [SerializeField] private AudioClip ratSplat;

    [Header("Music Player")]
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Player")]
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayIntroMusic()
    {
        if (musicSource && introMusic)
        {
            musicSource.clip = introMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayGameplayMusic()
    {
        if (musicSource && gameplayMusic)
        {
            musicSource.clip = gameplayMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayCatMeow() => PlaySFX(catMeow);
    public void PlayRatSplat() => PlaySFX(ratSplat);

    private void PlaySFX(AudioClip clip)
    {
        if (clip && sfxSource)
            sfxSource.PlayOneShot(clip);
    }
}