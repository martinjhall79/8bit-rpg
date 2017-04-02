/// <summary>
/// Player.
/// Capture keyboard inputs to control the player.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private MapMovementController moveController;
    // Start and stop animation when player begins and ends moving
    private Animator animator;

    void Start()
    {
        moveController = GetComponent<MapMovementController>();
        // Start and stop animation when player begins and stops moving
        moveController.moveActionCallback += OnMove;
        moveController.tileActionCallback += OnTile;
        animator = GetComponent<Animator>();
        animator.speed = 0;
    }

    // Start animation when player begins moving
    void OnMove()
    {
        animator.speed = 1;
    }

    // Stop animation when player ends movement and lands on a new tile
    void OnTile(int type)
    {
        animator.speed = 0;
    }

    // Workout direction to move player
    void Update()
    {
        // Store direction to move player to
        var dir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir.x = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir.x = -1;
        }

        // Only trigger move when there's a change in the direction
        if (dir.x != 0 || dir.y != 0)
        {
            moveController.MoveInDirection(dir);
        }
    }
}
