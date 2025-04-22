// MinionAI_ByTag.cs
// — Move em direção a minions de tag oposta.
// — Para e ataca em alcance, aplica dano via MinionHealth.
// — Usa IdleBattle entre ataques e Walk✱/Fly✱ fora de alcance.
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MinionInfo))]
[RequireComponent(typeof(MinionHealth))]

public class MinionAI_ByTag : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    MinionInfo info;
    MinionHealth health;
    string enemyTag;

    [Header("Ataque")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damageAmount = 10f;
    float lastAttackTime;

    const float moveThreshold = 0.1f;

    void Start()
    {
        agent  = GetComponent<NavMeshAgent>();
        anim   = GetComponent<Animator>();
        info   = GetComponent<MinionInfo>();
        health = GetComponent<MinionHealth>();

        anim.Play("IdleNormal");

        if (!agent.isOnNavMesh &&
            NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        if (tag == "PlayerMinion")      enemyTag = "EnemyMinion";
        else if (tag == "EnemyMinion")  enemyTag = "PlayerMinion";
        else enabled = false;
    }

    void Update()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return;

        var nearestObj = enemies
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .First();
        float dist = Vector3.Distance(transform.position, nearestObj.transform.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            var targetHealth = nearestObj.GetComponent<MinionHealth>();
            HandleAttack(targetHealth);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(nearestObj.transform.position);
            UpdateMovementAnimation(agent.velocity);
        }
    }

    void HandleAttack(MinionHealth targetHealth)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            anim.Play("IdleBattle");
            return;
        }
        lastAttackTime = Time.time;

        int idx = Random.Range(1, 4);
        anim.Play($"Attack0{idx}");

        if (targetHealth != null)
            targetHealth.TakeDamage(damageAmount);
    }

    void UpdateMovementAnimation(Vector3 velocity)
    {
        if (velocity.magnitude <= moveThreshold)
        {
            anim.Play("IdleNormal");
            return;
        }

        Vector3 localVel = transform.InverseTransformDirection(velocity).normalized;
        bool fwd  = localVel.z >  0.5f;
        bool back = localVel.z < -0.5f;
        bool right= localVel.x >  0.5f;
        bool left = localVel.x < -0.5f;

        string prefix = info.isFlying ? "Fly" : "Walk";
        string dir = fwd ? "FWD"
                   : back? "BWD"
                   : right? "Right"
                   : left?  "Left"
                   : "FWD";

        anim.Play(prefix + dir);
    }
}
