//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyPatrol : MonoBehaviour
//{
//    [Header("Configuração da Patrulha")]
//    public float patrolRadius = 15f; // Quão longe ele pode ir num único movimento
//    public float waitTimeAtPoint = 2f; // Tempo que ele espera ao chegar no ponto

//    // Método público que o "Cérebro" vai chamar
//    public bool GetRandomPoint(Vector3 center, out Vector3 result)
//    {
//        // 1. Gera um ponto aleatório dentro de uma esfera imaginária
//        Vector3 randomPoint = center + Random.insideUnitSphere * patrolRadius;

//        // 2. Tenta encontrar o ponto válido no NavMesh mais próximo desse ponto aleatório
//        NavMeshHit hit;
//        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
//        {
//            result = hit.position;
//            return true;
//        }

//        // Se não achou ponto válido (ex: gerou dentro de uma parede), retorna falso
//        result = Vector3.zero;
//        return false;
//    }
//}

using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [HideInInspector] public float patrolRadius;
    [HideInInspector] public float waitTimeAtPoint;

    public bool GetRandomPoint(Vector3 center, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
}