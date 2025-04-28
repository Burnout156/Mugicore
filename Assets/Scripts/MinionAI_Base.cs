using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MinionInfo))]
[RequireComponent(typeof(MinionHealth))]
public abstract class MinionAI_Base : MonoBehaviour
{
    /* ------- Expostos -------- */
    public NavMeshAgent agent;
    public Animator     anim;
    public MinionInfo   info;

    [Header("Distâncias")]
    public float stopDistance   = 50f;
    public float attackDistance = 45f;
    public float visionRadius   = 60f;

    [Header("Ataque")]
    public float attackCooldown = 1.2f;
    public float damageAmount   = 10f;

    [Header("Debug")]
    public bool  debugLogs      = true;
    public bool  traceDistances = true;

    /* ------- Estado interno --- */
    protected Transform target;
    protected bool      inCombat;
    float               lastAttack;

    /* ------- Quantidade de clipes de ataque ------ */
    protected virtual int MaxAttackIndex => 3;   // Ground = 3  | Fly sobrescreve p/ 2

    /* --------------------------- */
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim  = GetComponent<Animator>();
        info  = GetComponent<MinionInfo>();

        agent.autoBraking      = false;
        agent.stoppingDistance = stopDistance;

        if (!agent.isOnNavMesh &&
            NavMesh.SamplePosition(transform.position, out var hit, 4f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        anim.Play("IdleNormal");
    }

    /* --------------------------- */
    void Update()
    {
        if (traceDistances)
            Debug.Log($"{name,-10}| dist:{DistToTarget(),5:F1} | speed:{agent.velocity.magnitude,5:F1}");

        UpdateMovementAnimation(agent.velocity);

        if (!ValidTarget()) target = FindClosestEnemy();
        if (!target) return;

        /* --------- CHASE --------- */
        if (!inCombat)
        {
            if (!agent.hasPath) agent.SetDestination(target.position);

            if (DistToTarget() <= stopDistance)
            {
                inCombat = true;
                agent.isStopped = true; agent.ResetPath();
                anim.SetBool("InBattle", true);
            }
        }
        /* -------- COMBAT --------- */
        else
        {
            if (DistToTarget() > stopDistance)
            {
                inCombat = false;
                anim.SetBool("InBattle", false);
                agent.isStopped = false;
            }
            else
            {
                FaceTarget();
                TryAttack();
            }
        }

        ExtraUpdate();             // hook opcional (usado só no Fly)
    }

    /* ---------- ATAQUE ---------- */
    void TryAttack()
    {
        if (Time.time - lastAttack < attackCooldown) return;
        if (DistToTarget() > attackDistance) return;

        lastAttack = Time.time;

        int idx = Random.Range(1, MaxAttackIndex + 1);   // 1..2 ou 1..3
        anim.SetInteger("AttackIdx", idx);
        anim.SetTrigger("DoAttack");

        target.GetComponent<MinionHealth>()?.TakeDamage(damageAmount);
    }

    /* ---------- Helper abstratos ---------- */
    protected abstract void UpdateMovementAnimation(Vector3 velocity);
    protected virtual   void ExtraUpdate() { }   // Ground não usa

    /* ---------- Helper comuns ------------- */
    public float DistToTarget() => target ? Vector3.Distance(transform.position, target.position) : Mathf.Infinity;

    bool  ValidTarget()  => target && target.gameObject.activeSelf &&
                            (target.position - transform.position).sqrMagnitude < visionRadius * visionRadius;

    Transform FindClosestEnemy() =>
        FindObjectsOfType<MinionInfo>()
        .Where(m => m.ownerIsPlayer != info.ownerIsPlayer)
        .OrderBy(m => (m.transform.position - transform.position).sqrMagnitude)
        .Select(m => m.transform)
        .FirstOrDefault();

    void FaceTarget()
    {
        if (!target) return;
        Vector3 dir = target.position - transform.position; dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
