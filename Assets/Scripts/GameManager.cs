using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Player player;
    [SerializeField] private Transform levelBottomLeft;
    [SerializeField] private Transform levelTopRight;
    [SerializeField] private UIController uiController;
    [SerializeField] private WorkshopController workshopController;

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

    public Player Player => player;
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
            _currentWorkshopArea = value;
            PlayerInWorkshop = value != null;
        }
    }

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
        SetState(GameState.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) ||
            player.transform.position.y < levelBottomLeft.position.y ||
            player.transform.position.x < levelBottomLeft.position.x ||
            player.transform.position.y > levelTopRight.position.y ||
            player.transform.position.x > levelTopRight.position.x)
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
        Player.componentsConfig.LoadFromSaves(save.player.attachedComponents);
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
        if (newState == GameState.Workshop)
        {
            if (!_playerIsInWorkshop)
            {
                Debug.LogWarning("Tried to enter workshop state while not in workshop zone");
                return;
            }
        }

        var oldValue = _state;
        _state = newState;
        StateChanged?.Invoke(oldValue, _state);
    }

    void RespawnPlayer()
    {
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;
        var rbs = player.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }
}