using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Agent {

    public SpriteRenderer hatRenderer;
    public SpriteRenderer faceRenderer;
    public SpriteRenderer bodyRenderer;
    public Tile carryingTile;
    string humanName;

    public void BeBorn(string humanName_, Sprite hat, Sprite face, Sprite body) {
        humanName = humanName_;
        hatRenderer.sprite = hat;
        faceRenderer.sprite = face;
        bodyRenderer.sprite = body;
    }

    public void Die() {
        MapGen.instance.agents.Remove(this);
        Destroy(gameObject);
    }
}
