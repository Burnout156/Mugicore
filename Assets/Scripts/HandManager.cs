using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Refs")]
    public CardUI   cardPrefab;          // arraste o CardUI prefab
    public Transform handParent;         // mesmo GO do HorizontalLayout
    public List<CardData> shopPool;      // arraste TODOS os CardData aqui

    [Header("Config")]
    public int cardsPerShop = 6;

    void Start() => RollShop();

    public void RollShop()
    {
        // limpa mão anterior
        foreach (Transform c in handParent) Destroy(c.gameObject);

        // sorteia cartas
        for (int i = 0; i < cardsPerShop; i++)
        {
            CardData cd = shopPool[Random.Range(0, shopPool.Count)];
            var ui = Instantiate(cardPrefab, handParent);
            ui.Setup(cd);                 // mostra arte/nome
            ui.GetComponent<CardDrag>()?.Init(this);  // prepara drag (passo 4)
        }
    }
}
