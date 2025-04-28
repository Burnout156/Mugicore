using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardUI))]
public class CardClick : MonoBehaviour, IPointerClickHandler
{
    private CardUI cardUI;
    private HandManager hand;

    public void Init(HandManager h) => hand = h;

    void Awake() => cardUI = GetComponent<CardUI>();

    public void OnPointerClick(PointerEventData eventData)
    {
        // tenta spawnar no próximo slot livre
        if (hand.TrySpawnOnBench(cardUI.data.minionPrefab))
            Destroy(gameObject); // remove a carta da mão
    }
}
