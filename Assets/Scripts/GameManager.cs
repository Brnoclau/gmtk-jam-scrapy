using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Player player;
    [SerializeField] private Transform levelBottomLeft;
    [SerializeField] private Transform levelTopRight;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
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