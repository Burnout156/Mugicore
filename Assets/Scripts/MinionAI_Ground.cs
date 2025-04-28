using UnityEngine;

public class MinionAI_Ground : MinionAI_Base
{
    const float idleThresh = 0.05f;

    protected override void UpdateMovementAnimation(Vector3 vel)
    {
        float speed = vel.magnitude;
        anim.SetFloat("Speed", speed);          // Idle ↔ Walk
        if (speed <= idleThresh) return;
    }
}
