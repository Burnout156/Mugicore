using UnityEngine;
using UnityEngine.AI;

public class MinionAI_Fly : MinionAI_Base
{
    const float idleThresh = 0.05f;
    protected override int MaxAttackIndex => 2;   // só 2 clipes

    protected override void UpdateMovementAnimation(Vector3 vel)
    {
        float speed = vel.magnitude;
        anim.SetFloat("Speed", speed);          // Idle ↔ Fly
        if (speed <= idleThresh) return;
    }

    /* repath automático se travar */
    protected override void ExtraUpdate()
    {
        if (inCombat) return;
        if (DistToTarget() <= stopDistance) return;
        if (agent.velocity.sqrMagnitude > 0.01f) return;

        if (NavMesh.SamplePosition(target.position, out var hit, 8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
