using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlantSeeds : State {
    public int visionRange;
    public Navigate navigate;
    public bool failed = false;
    Dirt dirt;
    public override void Enter() {
        failed = false;
        //find dead dirt nearby
        dirt = FindDirt();

        if (dirt == null) {
            complete = true;
            failed = true;
            return;
        }
        navigate.target = dirt.pos;
        machine.Set(navigate, false);
    }


    public override void Tick() {
        if (state == null) {
            failed = true;
            complete = true;
            return;
        }
        if (state.complete) {
            if (dirt != null) {

                if (state == navigate && agent.pos == dirt.pos) {
                    //plant seeds
                    Plant();
                    complete = true;

                }
            } else {
                failed = true;
                complete = true;
            }
        }
    }


    public override void Exit() {
    }

    public Dirt FindDirt() {
        //find dead dirt nearby
        MapGen map = MapGen.instance;
        int radius = visionRange / 2;

        List<Tile> potentialTiles = map.FindTilesOfTypeInRadius(agent.pos, radius, typeof(Dirt));

        //use linq query to get a random unseeded dirt, if there are any
        if (potentialTiles.Count > 0) {
            List<Dirt> dirt = potentialTiles.Select(x => x as Dirt).ToList();
            dirt = dirt.Where(x => x.IsUnseeded()).ToList();
            if (dirt.Count > 0) {
                return dirt[Random.Range(0, dirt.Count)];
            }
        }

        return null;

    }

    public void Plant() {
        MapGen.instance.ReplaceTile(dirt.pos, new Wood(dirt.pos));
        //dirt.Plant();
    }
}
