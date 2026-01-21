using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float damage;

    public void Setup(float damageAmount, float speed)
    {
        this.damage = damageAmount;
        // Destroi a bala automaticamente após 5 segundos para não pesar o jogo
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Aqui futuramente você colocará a lógica de tirar vida do Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Acertei o Player! Dano: {damage}");
            HealthSystem playerHP = collision.gameObject.GetComponent<HealthSystem>();
            playerHP.TakeDamage(damage);
        }

        // Destroi a bala ao bater em qualquer coisa
        Destroy(gameObject);
    }
}