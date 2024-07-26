using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {
    [HideInInspector]
    public bool complete;
    [HideInInspector]
    public float startTime;
    [HideInInspector]
    public Vector2Int initPos;
    public float t => MapGen.instance.tickTime - startTime;

    public StateMachine machine;

    public Farmer agent => machine.agent;
    public State state => machine.state;

    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();

}
