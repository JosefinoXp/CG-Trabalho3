using UnityEngine;

public class DamageScreenEffect : MonoBehaviour
{
    [Header("Configurações do Efeito")]
    public Renderer quadRenderer; // Arraste o Quad aqui no Inspector
    public Color damageColor = new Color(1f, 0f, 0f, 0f); // Vermelho puro

    [Range(0f, 1f)]
    public float maxIntensity = 0.6f; // Limite máximo do alpha (para não cobrir 100% da visão)

    // Função chamada pelo HealthSystem
    public void UpdateDamageOverlay(float currentHP, float maxHP)
    {
        if (quadRenderer == null) return;

        // 1. Calcula a porcentagem de vida (0.0 a 1.0)
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);

        // 2. Inverte o valor (Vida baixa = Alpha alto)
        // Se vida for 100% (1.0), alpha será 0.
        // Se vida for 0% (0.0), alpha será maxIntensity.
        float targetAlpha = (1f - healthPercent) * maxIntensity;

        // 3. Cria a nova cor mantendo o vermelho original, mudando apenas o Alpha
        Color newColor = damageColor;
        newColor.a = targetAlpha;

        // 4. Aplica ao material (usando _Color para shaders Built-in padrão)
        quadRenderer.material.SetColor("_Color", newColor);
    }
}