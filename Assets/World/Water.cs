using UnityEngine;

public class Water : Tile {
    int health;
    int waterIndex;
    public Water(Vector2Int pos) {
        health = 100;
        this.pos = pos;
        spriteIndex = Random.Range(0, MapGen.instance.waterSprites.Length);
    }
    public override void Tick() {
        //do nothing
    }
    public override void Tock() {

    }

    public override Color GetColor() {
        return (Color.cyan + Color.blue) / 2f;
    }

    public override int GetSpriteIndex() {
        return spriteIndex + waterSpriteOffset;
    }

}