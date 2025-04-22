using UnityEngine;
using UnityEngine.AI;

public class Summoner : MonoBehaviour
{
    [Header("Minion Spawn Settings")]
    public GameObject minionPrefab;
    public float spawnInterval = 2f;
    public bool isPlayerSide = true;

    [Header("Minion Movement Settings")]
    [Tooltip("Velocidade de deslocamento do NavMeshAgent")]
    public float minionSpeed = 3.5f;
    [Tooltip("Taxa de aceleração do NavMeshAgent")]
    public float minionAcceleration = 8f;
    [Tooltip("Velocidade angular de virada do NavMeshAgent")]
    public float minionAngularSpeed = 120f;

    private float timer;

    private void Start()
    {
        SpawnMinion();
    }

    void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= spawnInterval)
        //{
        //    SpawnMinion();
        //    timer = 0f;
        //}
    }

    void SpawnMinion()
    {
        // Spawn sobre o NavMesh
        Vector3 pos = transform.position;
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            pos = hit.position;

        // Instancia o minion
        GameObject m = Instantiate(minionPrefab, pos, Quaternion.identity);
        m.name = isPlayerSide ? "Minion_P" : "Minion_E";

        // Marca quem é o dono
        var info = m.AddComponent<MinionInfo>();
        info.ownerIsPlayer = isPlayerSide;

        // Adiciona e configura o NavMeshAgent
        var agent = m.GetComponent<NavMeshAgent>();
        agent.radius = 0.00637f;
        agent.height = 0.02f;
        agent.baseOffset = agent.height * 0.5f;
        agent.speed = minionSpeed;
        agent.acceleration = minionAcceleration;
        agent.angularSpeed = minionAngularSpeed;

        // Garante que o agente esteja sobre o NavMesh
        if (!agent.isOnNavMesh)
            agent.Warp(pos);

        // Anexa a IA (ByOwner ou ByTag conforme sua escolha)
        m.AddComponent<MinionAI_ByOwner>();
    }
}
