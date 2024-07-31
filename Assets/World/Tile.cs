using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile {
    public static int dirtSpriteOffset = 0;
    public static int waterSpriteOffset = 0;
    public static int woodSpriteOffset = 0;
    public static int stoneSpriteOffset = 0;
    public static int foundationSpriteOffset = 0;

    public Vector2Int pos;
    public List<Tile> neighbours;
    public bool collectable = true;
    public int spriteIndex;
    public void SetNeighbours(List<Tile> neighbours) {
        this.neighbours = neighbours;
    }

    public abstract void Tick();
    public abstract void Tock();

    public abstract Color GetColor();
    public abstract int GetSpriteIndex();

}
