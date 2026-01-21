using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] public float playerHP = 100;
    private float maxHP; // Precisamos saber qual é o máximo para calcular a %

    [Header("Effects")]
    public DamageScreenEffect damageEffect; // Referência ao novo script criado acima

    private void Start()
    {
        // Define o HP atual como o máximo ao iniciar o jogo
        maxHP = playerHP;

        // Garante que a tela comece transparente
        UpdateVisuals();
    }

    public void TakeDamage(float damage)
    {
        playerHP -= damage;
        UpdateVisuals(); // Atualiza a tela vermelha
    }

    public void RestoreHealt(float HP)
    {
        playerHP += HP;

        // Garante que não cure mais que o máximo
        if (playerHP > maxHP) playerHP = maxHP;

        UpdateVisuals(); // Atualiza a tela vermelha (diminui o vermelho)
    }

    // Função auxiliar para atualizar o efeito
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
            // Nota: É ideal garantir que isso só rode uma vez, mas mantive sua lógica original
            SceneDeath death = gameObject.GetComponent<SceneDeath>();
            if (death != null) death.PlayerDeath();
        }
    }
}