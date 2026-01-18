using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Componentes")]
    public Transform spawnBullet; // Local de onde a bala sai (ponta da arma)
    public GameObject bulletPrefab; // O prefab da bala (precisa ter Rigidbody e EnemyBullet)

    [Header("Configurações da Bala")]
    public float speedBullet = 20f;
    public float sizeBullet = 1f;   // Escala da bala (1 = normal)
    public float damageBullet = 10f;

    [Header("Combate")]
    public float radiusBullet = 15f;    // Alcance máximo do ataque
    public float intervalBullet = 1f;   // Tempo entre tiros
    [Range(0, 100)]
    public float precisionBullet = 90f; // 100 = Perfeito, 0 = Muito ruim

    private float shotTimer;

    private void Update()
    {
        // Conta o tempo para o próximo tiro
        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }
    }

    // Método chamado pelo Cérebro (EnemyController)
    public void TryShoot(Transform target)
    {
        // 1. Verifica se o tempo de recarga acabou
        if (shotTimer > 0) return;

        // 2. Verifica a distância (Raio de Ataque)
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > radiusBullet) return;

        // 3. Se passou nos testes, atira
        Shoot(target);

        // Reinicia o timer
        shotTimer = intervalBullet;
    }

    private void Shoot(Transform target)
    {
        if (bulletPrefab == null || spawnBullet == null) return;

        // --- CALCULO DA PRECISÃO ---
        // Direção perfeita para o player
        Vector3 directionToTarget = (target.position - spawnBullet.position).normalized;

        // Calcula o erro baseado na precisão (quanto menor a precisão, maior o ângulo de erro)
        // Se precisão for 100, erro é 0. Se for 0, erro máximo é 45 graus.
        float errorAmount = 45f * (1f - (precisionBullet / 100f));

        // Gera rotações aleatórias nos eixos X e Y
        float errorX = Random.Range(-errorAmount, errorAmount);
        float errorY = Random.Range(-errorAmount, errorAmount);

        // Aplica o erro à direção original
        Quaternion rotationError = Quaternion.Euler(errorX, errorY, 0);
        Vector3 finalDirection = rotationError * directionToTarget;

        // --- INSTANCIAÇÃO ---
        GameObject bulletObj = Instantiate(bulletPrefab, spawnBullet.position, Quaternion.LookRotation(finalDirection));

        // Ajusta tamanho
        bulletObj.transform.localScale = Vector3.one * sizeBullet;

        // Configura script da bala
        EnemyBullet bulletScript = bulletObj.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.Setup(damageBullet, speedBullet);
        }

        // Aplica velocidade física
        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = finalDirection * speedBullet;
        }
    }

    // Desenha o raio de ataque no editor para facilitar ajuste
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusBullet);
    }
}