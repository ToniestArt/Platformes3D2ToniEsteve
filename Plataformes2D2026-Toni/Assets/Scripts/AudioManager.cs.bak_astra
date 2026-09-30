using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource _audioSource;

    [SerializeField]private AudioClip _level1Soundtrack;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void StartSoundtrack()
    {
        _audioSource.clip = _level1Soundtrack;
        _audioSource.Play();
    }

    public void PauseSoundtrack()
    {
        _audioSource.Pause();
    }
}
