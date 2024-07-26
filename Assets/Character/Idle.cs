public class Idle : State {

    public float waitTime = 1;
    public override void Enter() {
    }

    public override void Tick() {
        if (t > waitTime) {
            complete = true;
        }
    }

    public override void Exit() {
    }
}