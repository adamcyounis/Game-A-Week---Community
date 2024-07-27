using UnityEngine;

public class Foundation : Tile {
    public Foundation(Vector2Int pos) {
        this.pos = pos;
    }
    public override Color GetColor() {
        return Color.black;
    }

    public override void Tick() {
    }

    public override void Tock() {
    }
}