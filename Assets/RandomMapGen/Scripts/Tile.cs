/// <summary>
/// Sides.
/// </summary>
using UnityEngine;
using System.Collections;
using System;
using System.Text;

public enum Sides
{
    Bottom,
    Right,
    Left,
    Top
}

public class Tile
{
    public int id = 0;
    public Tile[] neighbours = new Tile[4];
    public int autotileID;

    public void AddNeighbour(Sides side, Tile tile)
    {
        neighbours[(int)side] = tile;
        CalculateAutotileID();
    }

    public void RemoveNeighbour(Tile tile)
    {
        var total = neighbours.Length;
        for (var i = 0; i < total; i++)
        {
            if (neighbours[i] != null)
            {
                if (neighbours[i].id == tile.id)
                {
                    neighbours[i] = null;
                }
            }
        }
        CalculateAutotileID();
    }

    public void ClearNeighbours()
    {
        var total = neighbours.Length;
        for (var i = 0; i < total; i++)
        {
            var tile = neighbours[i];
            if (tile != null)
            {
                tile.RemoveNeighbour(this);
                neighbours[i] = null;
            }
        }
        CalculateAutotileID();
    }

    private void CalculateAutotileID()
    {
        var sideValues = new StringBuilder();
        foreach (Tile tile in neighbours)
        {
            sideValues.Append(tile == null ? "0" : "1");
        }

        autotileID = Convert.ToInt32(sideValues.ToString(), 2);

    }
}
