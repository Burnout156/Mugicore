using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Mugicore/Card")]
public class CardData : ScriptableObject
{
    public string   cardName;
    public Sprite   artwork;
    public GameObject minionPrefab;   // qual minion nasce ao jogar
    [Range(1,5)] public int cost = 1; // ouro ou mana (se quiser)
}
