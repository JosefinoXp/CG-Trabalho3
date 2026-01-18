//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;

//[RequireComponent(typeof(NavMeshAgent))]

//public class EnemyController : MonoBehaviour
//{
//    public Transform player;
//    public float updateSpeed = 0.1f; //how long until new calculation of new path

//    private NavMeshAgent agent;

//    private void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        StartCoroutine(FollowPlayer());
//    }

//    private IEnumerator FollowPlayer()
//    {
//        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

//        while (enabled)
//        {
//            agent.SetDestination(player.transform.position);

//            yield return wait;
//        }
//    }
//}

//using System.Collections;
//using UnityEngine;
//using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(EnemyVision))] // Depende do script de visão
//public class EnemyController : MonoBehaviour
//{
//    private NavMeshAgent agent;
//    private EnemyVision vision;

//    private Vector3 lastKnownPosition;
//    private bool isSearching; // Estado: está procurando o jogador na última posição?

//    private void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        vision = GetComponent<EnemyVision>();
//    }

//    private void Update()
//    {
//        // CASO 1: O inimigo está vendo o jogador agora
//        if (vision.canSeePlayer)
//        {
//            isSearching = false; // Não estamos procurando, estamos vendo!
//            lastKnownPosition = vision.playerTarget.position; // Atualiza a memória

//            agent.isStopped = false;
//            agent.SetDestination(vision.playerTarget.position);
//        }
//        // CASO 2: Não está vendo, mas tem uma posição na memória para investigar
//        else if (!isSearching && HasValidLastPosition())
//        {
//            // Ativa o modo busca
//            isSearching = true;
//            agent.isStopped = false;
//            agent.SetDestination(lastKnownPosition);
//        }
//        // CASO 3: Está no modo busca (andando até o ponto onde o player sumiu)
//        else if (isSearching)
//        {
//            // Verifica se chegou ao destino (com uma margem de erro pequena)
//            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
//            {
//                // Chegou no local, olhou em volta e não viu nada.
//                // Desiste e para.
//                isSearching = false;
//                agent.isStopped = true;
//                // Aqui você poderia adicionar uma rotação de "olhar para os lados" ou voltar a patrulhar
//            }
//        }
//    }

//    // Auxiliar para garantir que a posição (0,0,0) não seja usada incorretamente no início
//    private bool HasValidLastPosition()
//    {
//        return lastKnownPosition != Vector3.zero;
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))]       // Precisa do script de Visão
[RequireComponent(typeof(EnemyPatrol))] // Precisa do script de Patrulha
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyVision vision;
    private EnemyPatrol patrol;

    // Variáveis de Estado
    private Vector3 lastKnownPosition;
    private bool isSearching;    // Está indo verificar onde o player sumiu?
    private bool isPatrolling;   // Está andando aleatoriamente?
    private float patrolTimer;   // Timer para esperar um pouco antes de andar de novo

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<EnemyVision>();
        patrol = GetComponent<EnemyPatrol>();
    }

    private void Update()
    {
        // --- ESTADO 1: PERSEGUIÇÃO ---
        if (vision.canSeePlayer)
        {
            EngageTarget();
        }
        // --- ESTADO 2: BUSCA (INVESTIGAÇÃO) ---
        else if (HasValidLastPosition())
        {
            InvestigateLastPosition();
        }
        // --- ESTADO 3: PATRULHA ALEATÓRIA ---
        else
        {
            PatrolRoaming();
        }
    }

    // LÓGICA DE PERSEGUIÇÃO
    private void EngageTarget()
    {
        isSearching = false;
        isPatrolling = false;

        // Atualiza memória
        lastKnownPosition = vision.playerTarget.position;

        // Move
        agent.isStopped = false;
        agent.SetDestination(vision.playerTarget.position);
    }

    // LÓGICA DE INVESTIGAÇÃO
    private void InvestigateLastPosition()
    {
        // Se ainda não estava buscando, inicia a busca
        if (!isSearching)
        {
            isSearching = true;
            isPatrolling = false;
            agent.isStopped = false;
            agent.SetDestination(lastKnownPosition);
        }

        // Verifica se chegou no ponto de investigação
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            // Chegou onde o player sumiu e não viu nada.
            // Limpa a memória e volta a patrulhar.
            lastKnownPosition = Vector3.zero;
            isSearching = false;
        }
    }

    // LÓGICA DE PATRULHA
    private void PatrolRoaming()
    {
        // Se já tem um destino de patrulha e está andando
        if (isPatrolling)
        {
            // Verifica se chegou no destino aleatório
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                // Chegou. Espera um pouco antes de buscar novo ponto.
                patrolTimer += Time.deltaTime;

                if (patrolTimer >= patrol.waitTimeAtPoint)
                {
                    isPatrolling = false; // Pronto para buscar novo ponto
                    patrolTimer = 0f;
                }
            }
        }
        else
        {
            // Precisa de um novo ponto. Pede ao módulo RandomPatrolSystem
            Vector3 newPatrolPoint;

            // Tenta pegar um ponto num raio ao redor da posição ATUAL do inimigo
            if (patrol.GetRandomPoint(transform.position, out newPatrolPoint))
            {
                agent.SetDestination(newPatrolPoint);
                isPatrolling = true;
                agent.isStopped = false;
            }
        }
    }

    private bool HasValidLastPosition()
    {
        return lastKnownPosition != Vector3.zero;
    }
}