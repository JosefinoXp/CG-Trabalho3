//using System.Collections;
//using UnityEngine;

//public class EnemyVision : MonoBehaviour
//{
//    [Header("Configurações de Visão")]
//    public float radius = 10f; // Distância da visão (Raio da esfera)

//    [Header("Layers")]
//    public LayerMask targetMask;      // Layer do Player
//    public LayerMask obstructionMask; // Layer das Paredes/Obstáculos

//    [Header("Estado Atual")]
//    public bool canSeePlayer;
//    public Transform playerTarget;

//    private void Start()
//    {
//        StartCoroutine(FOVRoutine());
//    }

//    private IEnumerator FOVRoutine()
//    {
//        WaitForSeconds wait = new WaitForSeconds(0.2f);

//        while (true)
//        {
//            yield return wait;
//            FieldOfViewCheck();
//        }
//    }

//    private void FieldOfViewCheck()
//    {
//        // 1. Detecta se o player está dentro da esfera de raio X
//        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

//        if (rangeChecks.Length != 0)
//        {
//            Transform target = rangeChecks[0].transform;
//            Vector3 directionToTarget = (target.position - transform.position).normalized;
//            float distanceToTarget = Vector3.Distance(transform.position, target.position);

//            // 2. Lança o raio. Se NÃO bater em obstáculo, é porque está vendo o player.
//            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
//            {
//                canSeePlayer = true;
//                playerTarget = target;
//            }
//            else
//            {
//                // O player está perto, mas tem uma parede na frente
//                canSeePlayer = false;
//            }
//        }
//        else if (canSeePlayer)
//        {
//            // O player saiu da esfera de distância
//            canSeePlayer = false;
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow; // Cor Amarela para Visão
//        Gizmos.DrawWireSphere(transform.position, radius);
//    }
//}

using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    // [HideInInspector] faz a variável ser pública pro código, mas invisível no Unity
    [HideInInspector] public float radius;
    [HideInInspector] public LayerMask targetMask;
    [HideInInspector] public LayerMask obstructionMask;

    // Variáveis de leitura (essas podem ficar visíveis ou não, você decide)
    public bool canSeePlayer;
    public Transform playerTarget;

    private void Start() { StartCoroutine(FOVRoutine()); }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true) { yield return wait; FieldOfViewCheck(); }
    }

    private void FieldOfViewCheck()
    {
        // Lógica idêntica à anterior...
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 dir = (target.position - transform.position).normalized;
            float dst = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, dir, dst, obstructionMask))
            {
                canSeePlayer = true;
                playerTarget = target;
            }
            else { canSeePlayer = false; }
        }
        else if (canSeePlayer) { canSeePlayer = false; }
    }

    // O Gizmo agora deve ser desenhado pelo script principal para não duplicar, 
    // ou mantido aqui se preferir. Vou deixar aqui para garantir.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}