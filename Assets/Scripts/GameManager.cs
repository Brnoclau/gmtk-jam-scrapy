using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Scrapy.Player;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scrapy
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GlobalConfig globalConfig;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Player.Player playerPrefab;
        [SerializeField] private Transform levelBottomLeft;
        [SerializeField] private Transform levelTopRight;
        [SerializeField] private WorkshopController workshopController;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private List<WorkshopArea> workshopAreas;
        [SerializeField] private CreditsUI creditsUI;

        [Header("Camera settings")] [SerializeField]
        private float playingCameraZoom = 6;

        [SerializeField] private float workshopCameraZoom = 4;
        [SerializeField] private float dialogCameraZoom = 4;
        [SerializeField] private float changeZoomTime = 1;
        [SerializeField] private Ease changeZoomEase = Ease.OutQuad;

        public static GameManager Instance { get; private set; }

        public event Action<bool> GamePausedChanged;
        public event Action<GameState, GameState> StateChanged;
        public event Action PlayerEnteredWorkshop;
        public event Action PlayerExitedWorkshop;

        public bool IsGamePaused
        {
            get => _isGamePaused;
            set
            {
                _isGamePaused = value;
                GamePausedChanged?.Invoke(value);
                Time.timeScale = _isGamePaused ? 0 : 1;
            }
        }

        private bool _isGamePaused;

        public GameState State
        {
            get => _state;
            set => SetState(value);
        }

        private GameState _state;

        public Player.Player Player => _player;
        public WorkshopController WorkshopController => workshopController;

        public WorkshopArea LastUsedWorkshop { get; private set; }

        public Interactable NearbyInteractable
        {
            get => _nearbyInteractable;
            set
            {
                if (value == _nearbyInteractable) return;
                _nearbyInteractable = value;
                if (_nearbyInteractable) InteractableChanged?.Invoke(_nearbyInteractable);
                else InteractableChanged?.Invoke(_nearbyInteractable);
            }
        }

        public event Action<Interactable> InteractableChanged;
        private Interactable _nearbyInteractable;


        public bool PlayerInWorkshop
        {
            get => _playerIsInWorkshop;
            private set
            {
                if (value == _playerIsInWorkshop) return;
                _playerIsInWorkshop = value;
                if (_playerIsInWorkshop) PlayerEnteredWorkshop?.Invoke();
                else PlayerExitedWorkshop?.Invoke();
            }
        }

        private bool _playerIsInWorkshop = false;

        public WorkshopArea CurrentWorkshopArea
        {
            get => _currentWorkshopArea;
            set
            {
                // The player is destroyed when he enters workshop, and the trigger counts as he exits it.
                // We need to preserve current workshop
                if (_currentWorkshopArea != null && Player == null) return;
                _currentWorkshopArea = value;
                PlayerInWorkshop = value != null;
            }
        }

        private Player.Player _player;
        private WorkshopArea _currentWorkshopArea;


        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra GameManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            creditsUI.FinishedCredits += OnFinishedCredits;
        }

        private void Start()
        {
            LoadGame();
            RespawnPlayer();
            SetState(GameState.Playing);
            IsGamePaused = false;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }

        void Update()
        {
            if (State == GameState.Playing)
            {
                if (Input.GetKeyDown(KeyCode.G) ||
                    _player.transform.position.y < levelBottomLeft.position.y ||
                    _player.transform.position.x < levelBottomLeft.position.x ||
                    _player.transform.position.y > levelTopRight.position.y ||
                    _player.transform.position.x > levelTopRight.position.x)
                {
                    RespawnPlayer();
                    SfxManager.Instance.Play(GlobalConfig.Instance.audio.exitWorkshopOrRespawn, 0.1f);
                }
            }
        }

        public void LoadGame()
        {
            var save = SaveManager.Instance.LoadGame();
            QuestManager.Instance.LoadFromSave();
            if (save.lastUsedWorkshopKey != null && save.lastUsedWorkshopKey.Length > 0)
            {
                var workshop = workshopAreas.FirstOrDefault(x => x.Key == save.lastUsedWorkshopKey);
                if (workshop == null)
                {
                    Debug.LogError(
                        $"No workshop registered in GameManager with key {save.lastUsedWorkshopKey}. Will respawn at Start Point.");
                    save.lastUsedWorkshopKey = null;
                }
                else
                {
                    LastUsedWorkshop = workshop;
                }
            }
            // WorkshopController.SetAvailableComponents(save);
        }

        public void ExitGame()
        {
            SaveManager.Instance.SaveGame();
            Application.Quit();
        }

        public void SetState(GameState newState)
        {
            if (newState == _state) return;

            if (_state == GameState.Workshop)
            {
                SaveManager.Instance.SaveGame();
            }

            if (newState == GameState.Workshop)
            {
                if (!PlayerInWorkshop)
                {
                    Debug.LogWarning("Tried to enter workshop state while not in workshop zone");
                    return;
                }

                // _currentWorkshopArea resets when we respawn player
                var workshop = CurrentWorkshopArea;
                LastUsedWorkshop = workshop;
                SaveManager.Instance.CurrentSave.lastUsedWorkshopKey = workshop.Key;
                RespawnPlayer();
                _player.transform.position = workshop.PlayerHoldPosition;
                FreezePlayer();
                SfxManager.Instance.Play(GlobalConfig.Instance.audio.enterWorkshop, 0.1f);
            }
            else if (_state == GameState.Workshop && newState == GameState.Playing)
            {
                var workshop = CurrentWorkshopArea;
                RespawnPlayer();
                SfxManager.Instance.Play(GlobalConfig.Instance.audio.exitWorkshopOrRespawn, 0.1f);
                if (workshop != null)
                {
                    _player.transform.position = workshop.PlayerHoldPosition;
                }
            }
            else if (newState == GameState.Credits)
            {
                creditsUI.ShowCredits();
            }

            float newZoom = playingCameraZoom;
            switch (newState)
            {
                case GameState.Playing:
                    newZoom = playingCameraZoom;
                    break;
                case GameState.Workshop:
                    newZoom = workshopCameraZoom;
                    break;
                case GameState.Dialog:
                    newZoom = dialogCameraZoom;
                    break;
            }

            cinemachineCamera.DOKill();
            DOTween.To(
                () => cinemachineCamera.Lens.OrthographicSize,
                value => cinemachineCamera.Lens.OrthographicSize = value,
                newZoom, changeZoomTime).SetEase(changeZoomEase).SetTarget(cinemachineCamera);

            var oldValue = _state;
            _state = newState;
            StateChanged?.Invoke(oldValue, _state);
        }

        void RespawnPlayer()
        {
            if (_player != null)
            {
                var toDestroy = _player;
                _player = null;
                Destroy(toDestroy.gameObject);
            }

            _player = Instantiate(playerPrefab);

            _player.transform.position = GetRespawnPosition();
            _player.LoadFromSaves(SaveManager.Instance.CurrentSave.player.attachedComponents);
            cinemachineCamera.Follow = _player.transform;
        }

        private Vector3 GetRespawnPosition()
        {
            if (LastUsedWorkshop != null) return LastUsedWorkshop.PlayerHoldPosition;
            return startPoint.localPosition;
        }

        private void OnFinishedCredits()
        {
            // State = GameState.Playing;
            SceneLoadingManager.Instance.LoadScene("Main Menu");
        }

        public void FreezePlayer()
        {
            var rbs = _player.GetComponentsInChildren<Rigidbody2D>();
            foreach (var rb in rbs)
            {
                // rb.simulated = false;

                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            foreach (var playerAttachedComponent in _player.AttachedComponents)
            {
                playerAttachedComponent.component.enabled = false;
            }
        }
    }
}