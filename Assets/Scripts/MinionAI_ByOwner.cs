using UnityEngine;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// MinionAI_ByOwner
///  — Move o minion até o inimigo mais próximo (baseado em MinionInfo.ownerIsPlayer).
///  — Para bruscamente a uma distância de “stopDistance”.
///  — Quando dentro de “attackDistance”, dispara ataques com cooldown.
///  — Fora de alcance de ataque, fica em “IdleBattle”.
///  — Fora de stopDistance, toca Walk✱ ou Fly✱ conforme isFlying.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MinionInfo))]
[RequireComponent(typeof(MinionHealth))]
public class MinionAI_ByOwner : MonoBehaviour
{
    NavMeshAgent agent;
    Animator     anim;
    MinionInfo   info;

    [Header("Distâncias")]
    public float stopDistance   = 10f;
    public float attackDistance = 10.5f;

    [Header("Ataque")]
    public float attackCooldown = 1.5f;
    public float damageAmount   = 10f;

    float lastAttackTime;
    const float moveThresh = 0.1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim  = GetComponent<Animator>();
        info  = GetComponent<MinionInfo>();

        agent.stoppingDistance = stopDistance;
        agent.autoBraking      = false;
    }

    void Start() => anim.Play("IdleNormal");

    void Update()
    {
        // procura alvo ------------------------------------------
        var enemy = FindObjectsOfType<MinionInfo>()
                    .Where(m => m.ownerIsPlayer != info.ownerIsPlayer)
                    .OrderBy(m => (m.transform.position - transform.position).sqrMagnitude)
                    .Select(m => m.transform)
                    .FirstOrDefault();
        if (!enemy) return;

        float dist = Vector3.Distance(transform.position, enemy.position);

        /* 1) manda mover só se ainda não tem caminho ou se precisamos recalcular   */
        if (!agent.hasPath || agent.remainingDistance > stopDistance)
            agent.SetDestination(enemy.position);

        /* 2) DEBUG simples ------------------------------- */
        if (dist < 15f)
            Debug.Log($"[{name}] dist:{dist:F1}  remain:{agent.remainingDistance:F1}  stop:{stopDistance:F1}");

        /* 3) se caminho finalizado e perto o suficiente -- */
        if (!agent.pathPending &&
            agent.remainingDistance != Mathf.Infinity &&
            agent.remainingDistance <= stopDistance)
        {
            agent.isStopped = true;

            if (dist <= attackDistance)
                TryAttack(enemy.GetComponent<MinionHealth>());
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
            anim.Play("IdleNormal");   // em cooldown
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
