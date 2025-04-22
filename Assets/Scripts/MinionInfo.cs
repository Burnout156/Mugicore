// MinionInfo:
//  — Armazena a quem o minion pertence (jogador ou inimigo) e se ele é voador.
//  — Use o Inspector para marcar “isFlying” nos prefabs de minions que voam.
using UnityEngine;

public class MinionInfo : MonoBehaviour
{
    [Header("Configuração de Propriedades")]
    [Tooltip("True = este minion voa; False = este minion anda no chão")]
    public bool isFlying = false;

    [Tooltip("True = pertence ao jogador; False = é inimigo")]
    public bool ownerIsPlayer = true;
}
