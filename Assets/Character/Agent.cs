using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Agent : MonoBehaviour {
    public bool ready = false;
    public Vector2Int pos;
    protected State state => machine.state;
    public int birthTick;
    public int age => (int)(((MapGen.instance.tickTime) - birthTick) / 36);
    public StateMachine machine;
    public virtual void Tick() { }


    public Tile FindTileOfType(Type type, int visionRange) {
        //find dead dirt nearby
        MapGen map = MapGen.instance;
        int radius = visionRange / 2;

        List<Tile> potentialTiles = map.FindTilesOfTypeInRadius(pos, radius, type);

        //use linq query to get a random unseeded dirt, if there are any
        if (potentialTiles.Count > 0) {
            return potentialTiles[UnityEngine.Random.Range(0, potentialTiles.Count)];
        }
        return null;

    }

    public void Update() {
        transform.position = MapGen.instance.MapToWorld(pos);
    }
}