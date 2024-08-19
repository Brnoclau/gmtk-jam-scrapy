using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapy
{
    public class SfxManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxAudioSource;
        
        public static SfxManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra SfxManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Play(AudioClip audioClip)
        {
            sfxAudioSource.PlayOneShot(audioClip);
        }
    }
}