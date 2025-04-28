using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("Refs")]
    public Image  artImage;
    public TMP_Text nameText;

    [HideInInspector] public CardData data;     // preenchido pelo Hand

    public void Setup(CardData cd)
    {
        data = cd;
        artImage.sprite = cd.artwork;
        nameText.text   = cd.cardName;
    }
}
