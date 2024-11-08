﻿using System;
using Scrapy.UI;
using Scrapy.UI.Workshop;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scrapy
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private FadeCanvas nonPauseUI;
        [SerializeField] private WorkshopUI workshopUI;
        [SerializeField] private FadeCanvas gameplayUI;
        [SerializeField] private FadeCanvas pauseUI;

        [Header("GamePlayUI")] [SerializeField]
        private Button toWorkshopButton;

        [Header("Pause UI")] [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button toMainMenuButton;
        [SerializeField] private Button exitGameButton;
        [SerializeField] private string mainMenuSceneName;
        
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private AudioClip sfxSoundOnSliderChange;

        private void Awake()
        {
            GameManager.Instance.StateChanged += OnStateChanged;
            // GameManager.Instance.PlayerEnteredWorkshop += OnPlayerEnteredWorkshop;
            // GameManager.Instance.PlayerExitedWorkshop += OnPlayerExitedWorkshop;
            GameManager.Instance.GamePausedChanged += OnPausedChanged;

            // OnPlayerExitedWorkshop();
            OnPausedChanged(GameManager.Instance.IsGamePaused);
            OnStateChanged(GameManager.Instance.State, GameManager.Instance.State);

            // toWorkshopButton.onClick.AddListener(() => GameManager.Instance.State = GameState.Workshop);

            resumeButton.onClick.AddListener(() => GameManager.Instance.IsGamePaused = false);
            saveButton.onClick.AddListener(() => SaveManager.Instance.SaveGame());
            toMainMenuButton.onClick.AddListener(ToMainMenu);
            exitGameButton.onClick.AddListener(() => GameManager.Instance.ExitGame());
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                exitGameButton.gameObject.SetActive(false);
            }
            
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);   
            musicSlider.onValueChanged.AddListener(SetMusicVolume);   
        }

        private void Start()
        {
            sfxSlider.SetValueWithoutNotify(OptionsManager.Instance.SfxVolume);
            musicSlider.SetValueWithoutNotify(OptionsManager.Instance.MusicVolume);
            nonPauseUI.SetOpen(true, true);
        }

        private void Update()
        {
            if (GameManager.Instance.State == GameState.Credits)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.IsGamePaused = !GameManager.Instance.IsGamePaused;
            }
        }

        private void ToMainMenu()
        {
            SaveManager.Instance.SaveGame();
            SceneLoadingManager.Instance.LoadScene(mainMenuSceneName);
        }

        void OnPausedChanged(bool isGamePaused)
        {
            pauseUI.SetOpen(isGamePaused);
            nonPauseUI.SetOpen(!isGamePaused);
        }

        void OnStateChanged(GameState oldState, GameState newState)
        {
            SetGameplayUIActive(newState == GameState.Playing);
            SetWorkshopUIActive(newState == GameState.Workshop);
        }

        void SetWorkshopUIActive(bool value)
        {
            workshopUI.SetOpen(value);
        }

        void SetGameplayUIActive(bool value)
        {
            gameplayUI.SetOpen(value);
        }

        // void OnPlayerEnteredWorkshop()
        // {
        //     toWorkshopButton.gameObject.SetActive(true);
        // }
        //
        // void OnPlayerExitedWorkshop()
        // {
        //     toWorkshopButton.gameObject.SetActive(false);
        // }

        private void SetSfxVolume(float value)
        {
            OptionsManager.Instance.SfxVolume = value;
            SfxManager.Instance.Play(sfxSoundOnSliderChange, 0.1f);
        }

        private void SetMusicVolume(float value)
        {
            OptionsManager.Instance.MusicVolume = value;
        }
    }
}