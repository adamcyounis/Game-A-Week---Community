using UnityEngine;

public class Navigate : State {

    public Vector2Int target;

    public override void Enter() {

    }

    public override void Tick() {
        if (agent.pos == target) {
            complete = true;
            return;
        } else {
            Vector2Int direction = target - agent.pos;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                direction.x = direction.x > 0 ? 1 : -1;
                direction.y = 0;
            } else {
                direction.x = 0;
                direction.y = direction.y > 0 ? 1 : -1;
            }
            agent.pos += direction;
        }
    }

    public override void Exit() {

    }
}

