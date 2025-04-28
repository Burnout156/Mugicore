using UnityEngine;

public class BoardSlot : MonoBehaviour
{
    [Header("Onde o minion nasce")]
    public Transform spawnPoint;

    public bool IsFree { get; private set; } = true;

    /// Spawna e marca como ocupado
    public void Spawn(GameObject prefab)
    {
        if (!IsFree || !prefab) return;
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        IsFree = false;
    }

    /// Se o minion morrer / sair do collider, libera o slot
    void OnTriggerExit(Collider other)
    {
        if (!IsFree &&
            other.TryGetComponent(out MinionHealth hp) &&
            hp.isDead)
            IsFree = true;
    }
}
