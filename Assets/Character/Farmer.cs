using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : Agent {

    public Navigate navigate;
    public Idle idle;
    public PlantSeeds plantSeeds;

    public int lifespan = 300;
    // Start is called before the first frame update
    public void Awake() {
        machine = new StateMachine(this);
        machine.Set(idle, true);
        ready = true;
    }


    // Update is called once per frame
    public override void Tick() {
        lifespan--;
        machine.Tick();

        if (state.complete) {
            if (state == idle || state == navigate) {
                machine.Set(plantSeeds, true);

            } else if (state == plantSeeds) {

                if (plantSeeds.failed) {
                    //navigate to random position
                    Navigate();
                } else {
                    machine.Set(idle, true);
                }
            }
        }

        if (lifespan <= 0) {
            MapGen.instance.agents.Remove(this);
            Destroy(gameObject);
        }

    }

    void Navigate() {
        int distance = 10;
        //target random map position
        navigate.target = new Vector2Int(Random.Range(0, MapGen.instance.size.x), Random.Range(0, MapGen.instance.size.y));
        machine.Set(navigate, true);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(MapGen.instance.MapToWorld(pos), Vector3.one * 0.1f);
    }

}
