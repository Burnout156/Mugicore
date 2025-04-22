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
     NavMeshAgent agent; Animator anim; MinionInfo info;
    string enemyTag;

    [Header("Distâncias")]
    public float stopDistance   = 2f;
    public float attackDistance = 1.5f;

    [Header("Ataque")]
    public float attackCooldown = 1.5f;
    public float damageAmount   = 10f;

    float lastAttackTime;
    const float moveThresh = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim  = GetComponent<Animator>();
        info  = GetComponent<MinionInfo>();

        agent.stoppingDistance = stopDistance;
        agent.autoBraking      = false;

        enemyTag = tag == "PlayerMinion" ? "EnemyMinion"
                  : tag == "EnemyMinion" ? "PlayerMinion"
                  : string.Empty;

        anim.Play("IdleNormal");
    }

    void Update()
    {
        if (enemyTag == string.Empty) return;
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return;

        var tgt   = enemies.OrderBy(e => (e.transform.position - transform.position).sqrMagnitude).First();
        float dist= Vector3.Distance(transform.position, tgt.transform.position);

        agent.SetDestination(tgt.transform.position);

        if (!agent.pathPending && dist <= stopDistance)
        {
            agent.isStopped = true;

            if (dist <= attackDistance)
                TryAttack(tgt.GetComponent<MinionHealth>());
            else
                anim.Play("IdleNormal");
        }
        else
        {
            agent.isStopped = false;
            UpdateMoveAnim(agent.velocity);
        }
    }

    void TryAttack(MinionHealth target)
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            anim.Play("IdleNormal");
            return;
        }
        lastAttackTime = Time.time;
        anim.Play($"Attack0{Random.Range(1,4)}");
        target?.TakeDamage(damageAmount);
    }

    void UpdateMoveAnim(Vector3 vel)
    {
        if (vel.magnitude <= moveThresh) { anim.Play("IdleNormal"); return; }

        Vector3 lv = transform.InverseTransformDirection(vel).normalized;
        string dir = lv.z >  0.5f ? "FWD" :
                     lv.z < -0.5f ? "BWD" :
                     lv.x >  0.5f ? "Right" : lv.x < -0.5f ? "Left" : "FWD";
        anim.Play((info.isFlying ? "Fly" : "Walk") + dir);
    }
}