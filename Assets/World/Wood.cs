using UnityEngine;

public class Wood : Tile {
    public Wood(Vector2Int pos) {
        this.pos = pos;
    }
    public override void Tick() {
    }

    public override void Tock() {
    }

    public override Color GetColor() {
        return (Color.green + Color.red * 2) / 5f;
    }
}