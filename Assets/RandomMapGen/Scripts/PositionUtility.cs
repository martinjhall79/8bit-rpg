/// <summary>
/// Position util.
/// Utility class that can be reused to calculate the position of GameObjects.
/// Calculates and index value and converts the index back to a position.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUtility
{
    // Calculate index
    public static void CalculateIndex(int x, int y, int width, out int index)
    {
        index = x + y * width;
    }
	
    // Convert index to x and y position
    public static void CalculatePosition(int index, int width, out int x, out int y)
    {
        x = index % width;
        y = index / width;
    }
}
