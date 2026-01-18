using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Configurações de Visão")]
    public float radius = 10f; // Distância da visão (Raio da esfera)

    [Header("Layers")]
    public LayerMask targetMask;      // Layer do Player
    public LayerMask obstructionMask; // Layer das Paredes/Obstáculos

    [Header("Estado Atual")]
    public bool canSeePlayer;
    public Transform playerTarget;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        // 1. Detecta se o player está dentro da esfera de raio X
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 2. Lança o raio. Se NÃO bater em obstáculo, é porque está vendo o player.
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                canSeePlayer = true;
                playerTarget = target;
            }
            else
            {
                // O player está perto, mas tem uma parede na frente
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            // O player saiu da esfera de distância
            canSeePlayer = false;
        }
    }
}