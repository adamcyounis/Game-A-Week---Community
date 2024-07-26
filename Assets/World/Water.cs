using UnityEngine;

public class Water : Tile {
    int health;

    public Water(Vector2Int pos) {
        health = 100;
        this.pos = pos;
    }
    public override void Tick() {
        //do nothing
    }
    public override void Tock() {

    }

    public override void Work() {

    }

    public override Color GetColor() {
        return (Color.cyan + Color.blue) / 2f;
    }

}