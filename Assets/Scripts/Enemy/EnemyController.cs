using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(EnemyGun))] // <--- Adicionamos dependência da Arma
public class EnemyController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Materiais de Estado")]
    [SerializeField] private Material patrolMaterial;
    [SerializeField] private Material chaseMaterial;
    [SerializeField] private Material investigateMaterial;

    private NavMeshAgent agent;
    private EnemyVision vision;
    private EnemyPatrol patrol;
    private EnemyGun gun; // <--- Referência para a arma

    // ... Variáveis de estado (lastKnownPosition, etc) ...
    private Vector3 lastKnownPosition;
    private bool isSearching;
    private bool isPatrolling;
    private float patrolTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<EnemyVision>();
        patrol = GetComponent<EnemyPatrol>();
        gun = GetComponent<EnemyGun>(); // <--- Pega o componente

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        // --- PERSEGUIÇÃO E COMBATE ---
        if (vision.canSeePlayer)
        {
            EngageTarget();
            UpdateColor(chaseMaterial);
        }
        else if (HasValidLastPosition())
        {
            InvestigateLastPosition();
            UpdateColor(investigateMaterial);
        }
        else
        {
            PatrolRoaming();
            UpdateColor(patrolMaterial);
        }
    }

    // ... (Método UpdateColor continua igual) ...
    private void UpdateColor(Material newMat)
    {
        if (meshRenderer != null && meshRenderer.sharedMaterial != newMat)
        {
            meshRenderer.material = newMat;
        }
    }

    private void EngageTarget()
    {
        isSearching = false;
        isPatrolling = false;
        lastKnownPosition = vision.playerTarget.position;

        agent.isStopped = false;
        agent.SetDestination(vision.playerTarget.position);

        // --- NOVA LINHA: TENTA ATIRAR ---
        // O script Gun vai cuidar sozinho se está no alcance ou se a arma está carregada
        gun.TryShoot(vision.playerTarget);
    }

    // ... (Os métodos InvestigateLastPosition e PatrolRoaming continuam iguais) ...
    private void InvestigateLastPosition()
    {
        if (!isSearching)
        {
            isSearching = true;
            isPatrolling = false;
            agent.isStopped = false;
            agent.SetDestination(lastKnownPosition);
        }

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            lastKnownPosition = Vector3.zero;
            isSearching = false;
        }
    }

    private void PatrolRoaming()
    {
        if (isPatrolling)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrol.waitTimeAtPoint)
                {
                    isPatrolling = false;
                    patrolTimer = 0f;
                }
            }
        }
        else
        {
            Vector3 newPatrolPoint;
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