using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapy
{
    public class SfxManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxAudioSource;
        
        public static SfxManager Instance { get; private set; }

        private readonly Dictionary<AudioClip, float> _recentPlayedClips = new();

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

        public void Play(AudioClip audioClip, float minTimeBetweenEffect = 0)
        {
            if (audioClip == null) return;
            Debug.Log("Playing audio clip: " + audioClip.name);
            if (minTimeBetweenEffect > 0 && _recentPlayedClips.ContainsKey(audioClip) &&
                _recentPlayedClips[audioClip] + minTimeBetweenEffect > Time.unscaledTime)
                return;
            sfxAudioSource.PlayOneShot(audioClip);
            if (minTimeBetweenEffect > 0)
            {
                _recentPlayedClips[audioClip] = Time.unscaledTime;
            }
        }
    }
}