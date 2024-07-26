using UnityEngine;

public class Stone : Tile {
    int minerals;

    public Stone(Vector2Int pos) {
        this.pos = pos;
        minerals = Random.Range(1, 10);
    }


    public override void Tick() {
        //do nothing
    }
    public override void Tock() {

    }
    public override void Work() {

    }
    public override Color GetColor() {
        //grey   
        return Color.gray;
    }

}
