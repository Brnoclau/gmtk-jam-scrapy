using System;
using UnityEngine;

public class WorkshopArea : MonoBehaviour
{
    [SerializeField] private Transform playerHoldPoint;
    
    private bool _playerInThisWorkshop = false;

    private void Awake()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        if (!_playerInThisWorkshop) return;
        var player = GameManager.Instance.Player;
        player.transform.position = playerHoldPoint.position;
        player.transform.rotation = playerHoldPoint.rotation;
    }

    void OnStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameState.Workshop && GameManager.Instance.CurrentWorkshopArea == this)
        {
            _playerInThisWorkshop = true;
        } else if (newState != GameState.Workshop)
        {
            _playerInThisWorkshop = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<Player>();
        if (player == null) return;
        GameManager.Instance.CurrentWorkshopArea = this;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<Player>();
        if (player == null) return;
        GameManager.Instance.CurrentWorkshopArea = null;
    }
}
