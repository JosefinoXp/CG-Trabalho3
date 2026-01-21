using UnityEngine;
using System.Collections; // Necessário para Corrotinas

public class HealthSystem : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] public float playerHP = 100;
    private float maxHP;

    [Header("Regeneration Config")]
    public float regenDelay = 7f;      // Tempo sem dano para começar a curar
    public float regenRate = 5f;       // Quanto de vida cura por "tick"
    public float regenFrequency = 0.1f; // A cada quanto tempo aplica a cura (menor = mais suave)

    [Header("Effects")]
    public DamageScreenEffect damageEffect;

    // Variável para guardar a referência da corrotina atual
    private Coroutine currentRegenCoroutine;

    private void Start()
    {
        maxHP = playerHP;
        UpdateVisuals();
    }

    public void TakeDamage(float damage)
    {
        playerHP -= damage;

        UpdateVisuals();

        // 1. Se já existir uma tentativa de regeneração rodando, PARE-A.
        // Isso "reseta" o contador de 7 segundos.
        if (currentRegenCoroutine != null)
        {
            StopCoroutine(currentRegenCoroutine);
        }

        // 2. Se o player ainda estiver vivo, inicie uma nova contagem para regenerar
        if (playerHP > 0)
        {
            currentRegenCoroutine = StartCoroutine(RegenHealthRoutine());
        }
    }

    public void RestoreHealt(float HP)
    {
        playerHP += HP;
        if (playerHP > maxHP) playerHP = maxHP;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (damageEffect != null)
        {
            damageEffect.UpdateDamageOverlay(playerHP, maxHP);
        }
    }

    public void Update()
    {
        if (playerHP <= 0)
        {
            // Para a regeneração se o player morrer
            if (currentRegenCoroutine != null) StopCoroutine(currentRegenCoroutine);

            SceneDeath death = gameObject.GetComponent<SceneDeath>();
            if (death != null) death.PlayerDeath();
        }
    }

    // --- NOVA CORROTINA DE REGENERAÇÃO ---
    private IEnumerator RegenHealthRoutine()
    {
        // Passo 1: Espera os 7 segundos (Delay)
        yield return new WaitForSeconds(regenDelay);

        // Passo 2: Começa a curar gradualmente
        while (playerHP < maxHP)
        {
            playerHP += regenRate;

            // Garante que não ultrapasse o máximo
            if (playerHP > maxHP) playerHP = maxHP;

            // Atualiza a tela vermelha (ela vai sumindo conforme cura)
            UpdateVisuals();

            // Espera um pouquinho antes de curar de novo (frequência)
            yield return new WaitForSeconds(regenFrequency);
        }

        // Limpa a referência quando terminar
        currentRegenCoroutine = null;
    }
}