/* Summoner.cs
 * — Spawna 1 minion no Start + outros a cada spawnInterval.
 * — Separa parâmetros de voo/solo e aplica NavMeshAgent.
 * — Injeta MinionInfo, MinionHealth e IA adequado.
 */
using UnityEngine;
using UnityEngine.AI;

public class Summoner : MonoBehaviour
{
    [Header("Prefab & Intervalo")]
    public GameObject minionPrefab;
    public float spawnInterval = 2f;
    public bool  isPlayerSide  = true;

    [Header("Settings – TERRESTRE")]
    public float groundSpeed        = 30f;
    public float groundAcceleration = 8f;
    public float groundAngularSpeed = 120f;

    [Header("Settings – VOADOR")]
    public float flySpeed        = 30f;
    public float flyAcceleration = 12f;
    public float flyAngularSpeed = 180f;
    public float flyHeight       = 3f;

    [Header("Combate")]
    public float stopDistance   = 10f;
    public float attackDistance = 10.5f;

    float timer;

    void Start()
    {
        if (!minionPrefab)
        {
            Debug.LogError("Summoner precisa do prefab!");
            enabled = false; return;
        }

        SpawnMinion();          // ← sempre gera 1 no começo
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnMinion();
            timer = spawnInterval;
        }
    }

    void SpawnMinion()
    {
        // 1) Descobre se o prefab é voador
        bool isFlyingPrefab = minionPrefab.GetComponent<MinionInfo>()?.isFlying ?? false;

        // 2) Posição inicial
        Vector3 pos = transform.position;
        if (!isFlyingPrefab &&
            NavMesh.SamplePosition(pos, out var hit, 2f, NavMesh.AllAreas))
            pos = hit.position;
        if (isFlyingPrefab)
            pos += Vector3.up * flyHeight;

        // 3) Instância + tag
        var m = Instantiate(minionPrefab, pos, Quaternion.identity);
        m.name = isPlayerSide ? "Minion_P" : "Minion_E";
        m.tag  = isPlayerSide ? "PlayerMinion" : "EnemyMinion";

        // 4) MinionInfo / Health
        var info = m.GetComponent<MinionInfo>() ?? m.AddComponent<MinionInfo>();
        info.ownerIsPlayer = isPlayerSide;
        if (!m.TryGetComponent(out MinionHealth _))
            m.AddComponent<MinionHealth>();

        // 5) NavMeshAgent
        var agent = m.GetComponent<NavMeshAgent>();
        if (isFlyingPrefab)
        {
            agent.speed        = flySpeed;
            agent.acceleration = flyAcceleration;
            agent.angularSpeed = flyAngularSpeed;
            agent.baseOffset   = flyHeight;
        }
        else
        {
            agent.speed        = groundSpeed;
            agent.acceleration = groundAcceleration;
            agent.angularSpeed = groundAngularSpeed;
            if (!agent.isOnNavMesh &&
                NavMesh.SamplePosition(pos, out var hit2, 1f, NavMesh.AllAreas))
                agent.Warp(hit2.position);
        }
        agent.stoppingDistance = stopDistance;
        agent.autoBraking      = false;

        // 6) IA
        if (isPlayerSide)   m.AddComponent<MinionAI_ByOwner>();
        else                m.AddComponent<MinionAI_ByTag>();
    }
}
