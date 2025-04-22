// MinionAI_ByOwner:
//  — Move o minion para o inimigo mais próximo (usando MinionInfo.ownerIsPlayer).
//  — Checa isFlying para tocar Walk✱ ou Fly✱ na direção certa.
//  — Desativa-se se MinionAI_ByTag estiver presente e previne duplicação.
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MinionInfo))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MinionHealth))]
public class MinionAI_ByOwner : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    MinionInfo info;
    MinionHealth health;

    [Header("Ataque")]
    [SerializeField, Tooltip("Distância mínima para parar e atacar")]
    private float attackRange = 26f;
    [SerializeField, Tooltip("Tempo entre ataques (segundos)")]
    private float attackCooldown = 1.5f;
    [SerializeField, Tooltip("Dano por ataque")]
    private float damageAmount = 10f;
    float lastAttackTime;

    const float moveThreshold = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        info = GetComponent<MinionInfo>();
        health = GetComponent<MinionHealth>();

        // faz o agente parar exatamente a 'attackRange' do destino
        agent.stoppingDistance = attackRange;

        anim.Play("IdleNormal");
    }

    void Update()
    {
        // encontra o inimigo mais próximo…
        var enemies = FindObjectsOfType<MinionInfo>()
            .Where(m => m.ownerIsPlayer != info.ownerIsPlayer)
            .Select(m => m.transform)
            .ToArray();
        if (enemies.Length == 0) return;

        var nearest = enemies
            .OrderBy(t => (t.position - transform.position).sqrMagnitude)
            .First();
        var targetHealth = nearest.GetComponent<MinionHealth>();

        // Distância exata ao alvo
        float distToTarget = Vector3.Distance(transform.position, nearest.position);

        // define destino
        agent.SetDestination(nearest.position);

        // ⬇️ Debug log para inspeção no Console
        Debug.Log($"[MinionAI_ByOwner] distToTarget: {distToTarget:F2}, remaining: {agent.remainingDistance:F2}, stopping: {agent.stoppingDistance:F2}"); // ← debug

        // se o agente estiver pronto e dentro do stoppingDistance:
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            HandleAttack(targetHealth);
        }
        else
        {
            agent.isStopped = false;
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
        bool fwd = localVel.z > 0.5f;
        bool back = localVel.z < -0.5f;
        bool right = localVel.x > 0.5f;
        bool left = localVel.x < -0.5f;

        string prefix = info.isFlying ? "Fly" : "Walk";
        string dirSuffix = fwd ? "FWD"
                         : back ? "BWD"
                         : right ? "Right"
                         : left ? "Left"
                         : "FWD";

        anim.Play(prefix + dirSuffix);
    }
}