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
    [HideInInspector] public float radius;
    [HideInInspector] public LayerMask targetMask;
    [HideInInspector] public LayerMask obstructionMask;

    public bool canSeePlayer;
    public Transform playerTarget;

    // Altura dos olhos do inimigo (Ajuste para ele não olhar do pé)
    private float eyeHeight = 1.6f;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true) { yield return wait; FieldOfViewCheck(); }
    }

    private void FieldOfViewCheck()
    {
        // 1. Detecta TODOS os colisores do player na área
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            // Assumimos falso até provar o contrário neste frame
            bool foundVisiblePart = false;

            // 2. Loop através de TODOS os colisores encontrados (Mão, Cabeça, Corpo)
            foreach (Collider col in rangeChecks)
            {
                Transform targetTransform = col.transform;

                // IMPORTANTE PARA XR: 
                // Usamos col.bounds.center em vez de targetTransform.position.
                // Isso garante que miramos no meio do corpo/cabeça, e não no pé (pivot do XR rig).
                Vector3 targetPosition = col.bounds.center;

                // Ponto de origem da visão (ajustado para altura dos olhos do inimigo, se necessário)
                // Se o pivot do inimigo for no chão, some Vector3.up * altura. 
                // Se o pivot já for na cabeça, use transform.position direto.
                Vector3 originPosition = transform.position + Vector3.up * 1.5f; // Ajuste 1.5f conforme altura do inimigo

                Vector3 directionToTarget = (targetPosition - originPosition).normalized;
                float distanceToTarget = Vector3.Distance(originPosition, targetPosition);

                // Lança o raio
                if (!Physics.Raycast(originPosition, directionToTarget, distanceToTarget, obstructionMask))
                {
                    // Achei pelo menos uma parte do corpo visível!
                    canSeePlayer = true;
                    playerTarget = targetTransform;
                    foundVisiblePart = true;

                    // Se já vi uma parte (ex: cabeça), não preciso checar o resto (ex: pé), economiza processamento
                    break;
                }
            }

            // Se rodou todos os colisores e todos estavam bloqueados por parede
            if (!foundVisiblePart)
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            // Jogador saiu do raio da esfera
            canSeePlayer = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Debug Visual do ponto de origem dos olhos (Opcional)
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.1f);
    }
}