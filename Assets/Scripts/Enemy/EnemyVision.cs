//using UnityEngine;
//using System.Collections;

//public class EnemyVision : MonoBehaviour
//{
//    [HideInInspector] public float radius;
//    [HideInInspector] public LayerMask targetMask;
//    [HideInInspector] public LayerMask obstructionMask;

//    public bool canSeePlayer;
//    public Transform playerTarget;

//    // Altura dos olhos do inimigo (Ajuste para ele não olhar do pé)
//    private float eyeHeight = 1.6f;

//    private void Start()
//    {
//        StartCoroutine(FOVRoutine());
//    }

//    private IEnumerator FOVRoutine()
//    {
//        WaitForSeconds wait = new WaitForSeconds(0.2f);
//        while (true) { yield return wait; FieldOfViewCheck(); }
//    }

//    private void FieldOfViewCheck()
//    {
//        // 1. Detecta TODOS os colisores do player na área
//        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

//        if (rangeChecks.Length != 0)
//        {
//            // Assumimos falso até provar o contrário neste frame
//            bool foundVisiblePart = false;

//            // 2. Loop através de TODOS os colisores encontrados (Mão, Cabeça, Corpo)
//            foreach (Collider col in rangeChecks)
//            {
//                Transform targetTransform = col.transform;

//                // IMPORTANTE PARA XR: 
//                // Usamos col.bounds.center em vez de targetTransform.position.
//                // Isso garante que miramos no meio do corpo/cabeça, e não no pé (pivot do XR rig).
//                Vector3 targetPosition = col.bounds.center;

//                // Ponto de origem da visão (ajustado para altura dos olhos do inimigo, se necessário)
//                // Se o pivot do inimigo for no chão, some Vector3.up * altura. 
//                // Se o pivot já for na cabeça, use transform.position direto.
//                Vector3 originPosition = transform.position + Vector3.up * 1.5f; // Ajuste 1.5f conforme altura do inimigo

//                Vector3 directionToTarget = (targetPosition - originPosition).normalized;
//                float distanceToTarget = Vector3.Distance(originPosition, targetPosition);

//                // Lança o raio
//                if (!Physics.Raycast(originPosition, directionToTarget, distanceToTarget, obstructionMask))
//                {
//                    // Achei pelo menos uma parte do corpo visível!
//                    canSeePlayer = true;
//                    playerTarget = targetTransform;
//                    foundVisiblePart = true;

//                    // Se já vi uma parte (ex: cabeça), não preciso checar o resto (ex: pé), economiza processamento
//                    break;
//                }
//            }

//            // Se rodou todos os colisores e todos estavam bloqueados por parede
//            if (!foundVisiblePart)
//            {
//                canSeePlayer = false;
//            }
//        }
//        else if (canSeePlayer)
//        {
//            // Jogador saiu do raio da esfera
//            canSeePlayer = false;
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, radius);

//        // Debug Visual do ponto de origem dos olhos (Opcional)
//        Gizmos.color = Color.blue;
//        Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.1f);
//    }
//}

using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    [HideInInspector] public float radius;
    [HideInInspector] public LayerMask targetMask;
    [HideInInspector] public LayerMask obstructionMask;

    [Header("Ajustes de Mira")]
    [Tooltip("Velocidade que o inimigo gira para encarar o player")]
    public float faceTargetSpeed = 8f;

    public bool canSeePlayer;
    public Transform playerTarget;

    // Altura dos olhos do inimigo
    private float eyeHeight = 1.6f;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    // --- NOVA PARTE: FORÇAR ROTAÇÃO ---
    private void Update()
    {
        // Se estou vendo o player e tenho um alvo válido
        if (canSeePlayer && playerTarget != null)
        {
            FaceTarget();
        }
    }

    private void FaceTarget()
    {
        // Calcula a direção para o player
        Vector3 direction = (playerTarget.position - transform.position).normalized;

        // IMPORTANTE: Zera o eixo Y para que o inimigo não olhe para o chão ou para o céu,
        // mantendo ele em pé retinho.
        direction.y = 0;

        // Se a direção for válida (não é zero)
        if (direction != Vector3.zero)
        {
            // Cria a rotação desejada
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Suaviza a rotação atual até a desejada (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * faceTargetSpeed);
        }
    }
    // ----------------------------------

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true) { yield return wait; FieldOfViewCheck(); }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            bool foundVisiblePart = false;

            foreach (Collider col in rangeChecks)
            {
                Transform targetTransform = col.transform;

                // Mira no centro do colisor (melhor para XR)
                Vector3 targetPosition = col.bounds.center;

                // Origem do raio (altura dos olhos)
                Vector3 originPosition = transform.position + Vector3.up * 1.5f;

                Vector3 directionToTarget = (targetPosition - originPosition).normalized;
                float distanceToTarget = Vector3.Distance(originPosition, targetPosition);

                if (!Physics.Raycast(originPosition, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    playerTarget = targetTransform;
                    foundVisiblePart = true;
                    break;
                }
            }

            if (!foundVisiblePart)
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.1f);
    }
}