using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile {
    public Vector2Int pos;
    public List<Tile> neighbours;

    public void SetNeighbours(List<Tile> neighbours) {
        this.neighbours = neighbours;
    }

    public abstract void Tick();
    public abstract void Tock();

    public abstract void Work();

    public abstract Color GetColor();

}
