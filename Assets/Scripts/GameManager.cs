using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Scrapy
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private Player.Player playerPrefab;
        [SerializeField] private Transform levelBottomLeft;
        [SerializeField] private Transform levelTopRight;
        [SerializeField] private UIController uiController;
        [SerializeField] private WorkshopController workshopController;
        [SerializeField] private CinemachineCamera cinemachineCamera;

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
                Debug.LogError("There should only be one GameManager in the scene");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            uiController.ToWorkshopTriggered += () => SetState(GameState.Workshop);
        }

        private void Start()
        {
            LoadGame();
            RespawnPlayer();
            SetState(GameState.Playing);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) ||
                _player.transform.position.y < levelBottomLeft.position.y ||
                _player.transform.position.x < levelBottomLeft.position.x ||
                _player.transform.position.y > levelTopRight.position.y ||
                _player.transform.position.x > levelTopRight.position.x)
            {
                RespawnPlayer();
            }
        }

        public void SaveGame()
        {
            SaveManager.Instance.SaveGame();
        }

        public void LoadGame()
        {
            var save = SaveManager.Instance.LoadGame();
            WorkshopController.SetAvailableComponents(save);
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
                SaveGame();
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
                RespawnPlayer();
                _player.transform.position = workshop.PlayerHoldPosition;
                FreezePlayer();
            } else if (newState == GameState.Playing)
            {
                var workshop = CurrentWorkshopArea;
                RespawnPlayer();
                if (workshop != null) 
                    _player.transform.position = workshop.PlayerHoldPosition;
            }

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
            
            _player.transform.position = respawnPoint.position;
            _player.transform.rotation = respawnPoint.rotation;
            _player.LoadFromSaves(SaveManager.Instance.CurrentSave.player.attachedComponents);
            cinemachineCamera.Follow = _player.transform;
        }

        public void FreezePlayer()
        {
            var rbs = _player.GetComponentsInChildren<Rigidbody2D>();
            foreach (var rb in rbs)
            {
                // rb.simulated = false;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }
}