using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {

    public Farmer agent;
    public State state;
    public StateMachine(Farmer agent) {
        this.agent = agent;
    }

    public void Tick() {
        //update the state
        state?.Tick();

        //call this function recursively on the state's statemachine
        state?.machine?.Tick();
    }

    public void Set(State newState, bool overRide) {
        if (state != newState || overRide) {
            if (state != null) {
                state.Exit();
            }

            state = newState;

            if (state.machine == null) {
                state.machine = new StateMachine(agent);
            }
            state.machine.agent = agent;
            state.startTime = Time.time;
            state.initPos = agent.pos;
            state.complete = false;

            state.Enter();
        }
    }
}
