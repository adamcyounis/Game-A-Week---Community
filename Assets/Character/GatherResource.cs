using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GatherResource<U> : State {
    public Navigate navigate;
    public Tile target;
    public override void Enter() {
        machine.state = null;
        //find the closest resource
        target = FindResource();
        if (target == null) {
            //find the nearest resource
            List<Tile> allTilesOfType = MapGen.instance.FindTilesOfTypeInRadius(agent.pos, 100, typeof(U));
            target = allTilesOfType.Where(x => x.collectable).OrderBy(x => Vector2Int.Distance(x.pos, agent.pos)).FirstOrDefault();
        }

        if (target != null) {
            navigate.target = target.pos;
            machine.Set(navigate, true);
        } else {
            complete = true;
        }
    }

    public override void Tick() {

        if (target != null && state.complete) {
            if (state == navigate) {
                if (agent.pos == navigate.target) {
                    //if it's stone or wood, replace with dirt
                    if (target is Stone || target is Wood) {

                        Tile replacementTile = agent.carryingTile;

                        if (replacementTile == null) {
                            replacementTile = new Dirt(target.pos);
                        }

                        agent.carryingTile = target;
                        MapGen.instance.ReplaceTile(target.pos, replacementTile);
                        complete = true;
                    }
                }
            }
        }
    }

    public override void Exit() {
    }

    public Tile FindResource() {
        MapGen map = MapGen.instance;
        int radius = 10;

        List<Tile> potentialTiles = map.FindTilesOfTypeInRadius(agent.pos, radius, typeof(U));

        //find only tiles that are collectable
        potentialTiles = potentialTiles.Where(x => x.collectable).ToList();

        if (potentialTiles.Count > 0) {
            return potentialTiles[UnityEngine.Random.Range(0, potentialTiles.Count)];
        }

        return null;
    }
}
