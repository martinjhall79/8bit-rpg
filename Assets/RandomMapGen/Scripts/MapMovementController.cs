/// <summary>
/// Map movement controller.
/// The logic to move any GameObject around the map
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapMovementController : MonoBehaviour
{
    public Map map;
    // Tile size dimensions
    public Vector2 tileSize;
    public int currentTile;
    // Movement animation
    public float speed = 1f;
    public bool moving;
    public float moveTime;
    // Limit movement to valid tiles
    public int[] blockedTileTypes;
    // Trigger events when player lands on tile, informing what the tile is
    public delegate void TileAction(int Type);
    public TileAction tileActionCallback;
    // Trigger event when player starts moving
    public delegate void MoveAction();
    public MoveAction moveActionCallback;

    private Vector2 startPos;
    private Vector2 endPos;

    private int tmpIndex;
    // Reusable x and y position to use in calculations for moving game objects around map
    private int tmpX;
    private int tmpY;


    public void MoveTo(int index, bool animate = false)
    {
        // Only move if movement valid
        if (!CanMove(index)) {
            return;
        }

        // Player can move, trigger beginning movement event
        if (moveActionCallback != null)
        {
            moveActionCallback();
        }

        currentTile = index;

        // Calculate new position
        PositionUtility.CalculatePosition(index, map.columns, out tmpX, out tmpY);

        // Calculate actual position in scene
        tmpX *= (int)tileSize.x;
        tmpY *= -(int)tileSize.y;

        var newPos = new Vector3(tmpX, tmpY, 0);
        // Move GameObject to new position
        if (!animate)
        {
            transform.position = newPos;
            // Trigger event when player lands on this tile
            if (tileActionCallback != null)
            {
                tileActionCallback(map.tiles[currentTile].autotileID);
            }
        }
        else {
            // Animate movement
            startPos = transform.position;
            endPos = newPos;
            moveTime = 0;
            moving = true;
        }
    }

    // Figure out the next tile to move to, based on direction from key press
    public void MoveInDirection(Vector2 dir)
    {
        PositionUtility.CalculatePosition(currentTile, map.columns, out tmpX, out tmpY);

        // Add direction values to temp x and y
        tmpX += (int)dir.x;
        tmpY += (int)dir.y;

        // Create index of tile to move to
        PositionUtility.CalculateIndex(tmpX, tmpY, map.columns, out tmpIndex);

        // Move player to next tile, with animation
        MoveTo(tmpIndex, true);
    }


    void Update()
    {
        // Movement animation
        if (moving)
        {
            moveTime += Time.deltaTime;
            // Stop moving player if animation finished
            if (moveTime > speed) 
            {
                moving = false;
                transform.position = endPos;
                // When animation complete, trigger event when player lands on this tile
                if (tileActionCallback != null)
                {
                    tileActionCallback(map.tiles[currentTile].autotileID);
                }
            }
            transform.position = Vector2.Lerp(startPos, endPos, moveTime / speed);
        }
    }

    // Limit player movement to valid parts of the map
    bool CanMove(int index)
    {
        // Make sure index is always in range
        if (index < 0 || index >= map.tiles.Length)
        {
            return false;
        }
        // Limit to valid tiles only
        var tileType = map.tiles[index].autotileID;
  
        // Only accept movement when player stopped
        // Prevent movement onto ocean and mountain tiles
        if (moving || Array.IndexOf(blockedTileTypes, tileType) > -1)  
        {
            return false;
        }
        return true;
    }
}
