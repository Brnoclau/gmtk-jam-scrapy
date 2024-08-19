using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapy
{
    public class OptionsManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        public static OptionsManager Instance { get; private set; }

        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;
                audioMixer.SetFloat("SFXVolume", Mathf.Log(_sfxVolume) * 20);
                SaveSettings();
            }
        }

        private float _sfxVolume;

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                audioMixer.SetFloat("MusicVolume", Mathf.Log(_musicVolume) * 20);
                SaveSettings();
            }
        }

        private float _musicVolume;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra OptionsManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }

        private void Start()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            var sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 0.5f);
            var musicVolume = PlayerPrefs.GetFloat("mus_volume", 0.5f);
            // get both values first because setting one of them saves both
            SfxVolume = sfxVolume;
            MusicVolume = musicVolume;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("sfx_volume", SfxVolume);
            PlayerPrefs.SetFloat("mus_volume", MusicVolume);
        }
    }
}