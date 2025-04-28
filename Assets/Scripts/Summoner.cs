/* Summoner.cs  –  Unity 2023.2
 * -----------------------------------------------------------
 * • Spawna 1 minion no Start e outros a cada spawnInterval
 * • Define Tag do minion  ➜  "PlayerMinion" / "EnemyMinion"
 * • Ajusta NavMeshAgent (solo × voo) e injeta IA (Ground / Fly)
 * -----------------------------------------------------------
 *  ⚠  CRIE as tags “PlayerMinion” e “EnemyMinion” em:
 *     Edit ▸ Project Settings ▸ Tags & Layers ▸ Tags ▸ + Add Tag
 */
using UnityEngine;
using UnityEngine.AI;

public class Summoner : MonoBehaviour
{
    /* -------- PREFAB & INTERVALO ---------------- */
    [Header("Prefab & Intervalo")]
    public GameObject minionPrefab;
    public float      spawnInterval = 0.5f;   // teste contínuo
    public bool       isPlayerSide  = true;

    /* -------- PARÂMETROS NAVMESH ---------------- */
    const float groundSpeed        = 20f,  groundAcc = 15f,  groundTurn = 720f;
    const float groundRadius       = 0.4f, groundHeight = 1.8f;
    const float flySpeed           = 20f,  flyAcc    = 24f, flyTurn    = 720f;
    const float flyHeight          = 1.8f;          // offset no ar

    /* -------- DISTÂNCIAS DE COMBATE ------------- */
    [Header("Combate")]
    public float stopDistance   = 50f;
    public float attackDistance = 45f;

    float timer;

    /* ------------- START ------------------------ */
    void Start()
    {
        if (!minionPrefab)
        {
            Debug.LogError("Summoner requer um prefab!");
            enabled = false; return;
        }

        SpawnMinion();                    // 1º minion imediato
        timer = spawnInterval;
    }

    /* ------------- UPDATE ----------------------- */
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnMinion();
            timer = spawnInterval;
        }
    }

    /* ------------- SPAWN ------------------------ */
    void SpawnMinion()
    {
        /* 1) prefab voa? */
        bool flies = minionPrefab.GetComponent<MinionInfo>()?.isFlying ?? false;

        /* 2) posição inicial */
        Vector3 pos = transform.position;
        if (flies)
            pos += Vector3.up * flyHeight;
        else if (NavMesh.SamplePosition(pos, out var snap, 2f, NavMesh.AllAreas))
            pos = snap.position;

        /* 3) instância + TAG */
        GameObject m = Instantiate(minionPrefab, pos, Quaternion.identity);
        m.name = (isPlayerSide ? "Minion_P" : "Minion_E") + $"_{Random.Range(0,999)}";
        m.tag  = isPlayerSide ? "PlayerMinion" : "EnemyMinion";   //  ← TAG definida

        /* 4) MinionInfo & Health */
        var info = m.GetComponent<MinionInfo>() ?? m.AddComponent<MinionInfo>();
        info.ownerIsPlayer = isPlayerSide;
        if (!m.TryGetComponent(out MinionHealth _))
            m.AddComponent<MinionHealth>();


        /* 6) injeta IA adequada */
        MinionAI_Base ai = flies
            ? m.AddComponent<MinionAI_Fly>()
            : m.AddComponent<MinionAI_Ground>();

  
    }
}
