// MinionHealth.cs
// — Controla a vida do minion, recebe dano e dispara evento de morte.
using UnityEngine;
using UnityEngine.AI;

public class MinionHealth : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Chame isso para aplicar dano
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        // Aqui você pode tocar animação de morte ou destruir o objeto
        var anim = GetComponent<Animator>();
        if (anim) anim.SetTrigger("Die");
        // Desativa navegação e AI
        var agent = GetComponent<NavMeshAgent>();
        if (agent) agent.isStopped = true;
        foreach(var ai in GetComponents<MonoBehaviour>())
            if (ai is NavMeshAgent == false && ai is Animator == false)
                ai.enabled = false;
        // Destrói após 3s (tempo para animação)
        Destroy(gameObject, 3f);
    }
}
