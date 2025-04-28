using UnityEngine;
using UnityEngine.EventSystems;

/// Arraste a carta da mão para um BoardSlot
public class CardDrag : MonoBehaviour,
                        IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform   originalParent;
    Canvas      canvas;
    CardUI      card;
    HandManager hand;

    public void Init(HandManager h) => hand = h;

    void Awake()
    {
        card   = GetComponent<CardUI>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData e)
    {
        transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        BoardSlot slot = null;

        if (e.pointerCurrentRaycast.gameObject)
            slot = e.pointerCurrentRaycast.gameObject.GetComponent<BoardSlot>();

        if (slot != null && slot.IsFree)
        {
            slot.Spawn(card.data.minionPrefab);
            Destroy(gameObject);             // remove carta da mão
        }
        else   // volta para mão
        {
            transform.SetParent(originalParent, false);
            transform.localPosition = Vector3.zero;
        }
    }
}
