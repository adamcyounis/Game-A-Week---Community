using UnityEngine;

public class Stone : Tile {
    int minerals;


    public Stone(Vector2Int pos) {
        this.pos = pos;
        minerals = Random.Range(1, 10);
        spriteIndex = Random.Range(0, MapGen.instance.stoneSprites.Length);
    }


    public override void Tick() {
        //do nothing
    }
    public override void Tock() {

    }
    public override Color GetColor() {
        //grey   
        return Color.gray;
    }

    public override int GetSpriteIndex() {
        return spriteIndex + stoneSpriteOffset;
    }

}
