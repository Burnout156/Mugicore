using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MinionInfo))]
public class MinionHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [HideInInspector] public bool isDead;   // ← consulta externa (IA)

    void Awake() => currentHealth = maxHealth;

    /* ---------- DANO ---------- */
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
            Die();
    }

    /* ---------- MORTE --------- */
    void Die()
    {
        isDead = true;

        /* animação: define bool IsDead */
        var anim = GetComponent<Animator>();
        if (anim) anim.SetBool("IsDead", true);

        /* interrompe navegação */
        var nav = GetComponent<NavMeshAgent>();
        if (nav) nav.isStopped = true;

        /* desativa todas as IAs */
        foreach (var mb in GetComponents<MonoBehaviour>())
            if (mb != this && !(mb is Animator) && !(mb is NavMeshAgent))
                mb.enabled = false;

        /* destrói após 3 s (tempo de clipe) */
        Destroy(gameObject, 3f);
    }
}
