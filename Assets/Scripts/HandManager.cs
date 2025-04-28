using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandManager : MonoBehaviour
{
    [Header("Refs (Mão)")]
    public CardUI   cardPrefab;
    public List<Transform> cardSlots;    // Slot0..Slot5 do hand

    [Header("Refs (Banco)")]
    public List<BoardSlot> benchSlots;   // BenchSlot0..BenchSlotN

    [Header("Pool de Cartas")]
    public List<CardData> shopPool;

    [Header("Config")]
    [Range(1,6)] public int cardsPerShop = 6;

    void Start()
    {
        RollShop();
    }

    public void RollShop()
    {
        // limpa cartas antigas
        foreach (var slot in cardSlots)
            foreach (Transform c in slot) Destroy(c.gameObject);

        // instancia até cardsPerShop cartas
        for (int i = 0; i < cardsPerShop && i < cardSlots.Count; i++)
        {
            var cd = shopPool[Random.Range(0, shopPool.Count)];
            var ui = Instantiate(cardPrefab, cardSlots[i]);
            ui.Setup(cd);
            var drag = ui.GetComponent<CardDrag>();
            if (drag != null) drag.Init(this);
            var click = ui.GetComponent<CardClick>();
            if (click != null) click.Init(this);
        }
    }

    /// Chamado por CardClick, retorna true se conseguiu spawnar
    public bool TrySpawnOnBench(GameObject prefab)
    {
        var slot = benchSlots.FirstOrDefault(s => s.IsFree);
        if (slot == null) return false;
        slot.Spawn(prefab);
        return true;
    }
}
