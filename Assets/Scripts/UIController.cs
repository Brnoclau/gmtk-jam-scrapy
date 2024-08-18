using System;
using Scrapy.UI.Workshop;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private WorkshopUI workshopUI;
        [SerializeField] private GameObject gameplayUI;
        [SerializeField] private GameObject pauseUI;

        [Header("GamePlayUI")] [SerializeField]
        private Button toWorkshopButton;

        [Header("Pause UI")] [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button exitGameButton;

        private void Awake()
        {
            GameManager.Instance.StateChanged += OnStateChanged;
            GameManager.Instance.PlayerEnteredWorkshop += OnPlayerEnteredWorkshop;
            GameManager.Instance.PlayerExitedWorkshop += OnPlayerExitedWorkshop;
            GameManager.Instance.GamePausedChanged += OnPausedChanged;

            OnPlayerExitedWorkshop();
            OnPausedChanged(GameManager.Instance.IsGamePaused);
            OnStateChanged(GameManager.Instance.State, GameManager.Instance.State);

            toWorkshopButton.onClick.AddListener(() => GameManager.Instance.State = GameState.Workshop);

            resumeButton.onClick.AddListener(() => GameManager.Instance.IsGamePaused = false);
            saveButton.onClick.AddListener(() => SaveManager.Instance.SaveGame());
            exitGameButton.onClick.AddListener(() => GameManager.Instance.ExitGame());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.IsGamePaused = !GameManager.Instance.IsGamePaused;
            }
        }

        void OnPausedChanged(bool isGamePaused)
        {
            pauseUI.gameObject.SetActive(isGamePaused);
        }

        void OnStateChanged(GameState oldState, GameState newState)
        {
            SetGameplayUIActive(newState == GameState.Playing);
            SetWorkshopUIActive(newState == GameState.Workshop);
        }

        void SetWorkshopUIActive(bool value)
        {
            workshopUI.gameObject.SetActive(value);
        }

        void SetGameplayUIActive(bool value)
        {
            gameplayUI.gameObject.SetActive(value);
        }

        void OnPlayerEnteredWorkshop()
        {
            toWorkshopButton.gameObject.SetActive(true);
        }

        void OnPlayerExitedWorkshop()
        {
            toWorkshopButton.gameObject.SetActive(false);
        }
    }
}