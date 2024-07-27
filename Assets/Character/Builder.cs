using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Agent {

    public Navigate navigate;
    public Idle idle;
    public int lifespan = 300;
    public BuildHouse buildHouse;
    //public GoHome goHome;

    // Start is called before the first frame update
    public void Awake() {
        machine = new StateMachine(this);
        machine.Set(buildHouse, true);
        ready = true;
    }

    // Update is called once per frame
    public override void Tick() {
        // lifespan--;
        machine.Tick();
        if (state.complete) {
            machine.Set(buildHouse, true);
        }

        if (lifespan <= 0) {
            MapGen.instance.agents.Remove(this);
            Destroy(gameObject);
        }

    }


    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(MapGen.instance.MapToWorld(pos), Vector3.one * 0.1f);
        /*
        #if UNITY_EDITOR
                UnityEditor.Handles.Label(MapGen.instance.MapToWorld(pos) + new Vector3(0, 0, -1), machine.GetStateBranchName("name | "));
        #endif
        */
    }
}
