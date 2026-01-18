using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Exige que os outros scripts estejam no objeto
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(EnemyGun))]
[RequireComponent(typeof(EnemyPatrol))]
public class Enemy : MonoBehaviour
{
    // ========================================================================
    // PAINEL DE CONTROLE (Tudo que você edita fica aqui)
    // ========================================================================

    #region 1. Visual & Referências
    [Header("--- VISUAL SETTINGS ---")]
    [Tooltip("Arraste o objeto com o MeshRenderer (corpo) aqui.")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material matPatrol;
    [SerializeField] private Material matInvestigate;
    [SerializeField] private Material matChase;
    #endregion

    #region 2. Configuração da Visão
    [Header("--- VISION SETTINGS ---")]
    [Tooltip("Distância máxima que a IA enxerga.")]
    [SerializeField] private float viewRadius = 20f;
    [Tooltip("O que bloqueia a visão? (Paredes, etc)")]
    [SerializeField] private LayerMask obstacleMask;
    [Tooltip("O que é o alvo? (Player)")]
    [SerializeField] private LayerMask targetMask;
    #endregion

    #region 3. Configuração da Arma
    [Header("--- WEAPON SETTINGS ---")]
    [Tooltip("Ponto de origem do tiro (ponta da arma).")]
    [SerializeField] private Transform firePoint;
    [Tooltip("O Prefab do projétil.")]
    [SerializeField] private GameObject bulletPrefab;

    [Space(5)]
    [Tooltip("Distância máxima para ATIRAR (Deve ser menor que a visão).")]
    [SerializeField] private float attackRange = 15f;
    [Tooltip("Intervalo entre tiros (segundos).")]
    [SerializeField] private float fireRate = 1f;
    [Tooltip("Precisão (0-100).")]
    [Range(0, 100)][SerializeField] private float accuracy = 90f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletSize = 1f;
    #endregion

    #region 4. Configuração da Patrulha
    [Header("--- PATROL SETTINGS ---")]
    [Tooltip("Raio de movimentação aleatória.")]
    [SerializeField] private float wanderRadius = 15f;
    [Tooltip("Tempo de espera ao chegar num ponto.")]
    [SerializeField] private float waitTime = 2f;
    #endregion

    // ========================================================================
    // VARIÁVEIS INTERNAS (CONTROLE)
    // ========================================================================
    private NavMeshAgent agent;
    private EnemyVision visionModule;
    private EnemyGun gunModule;
    private EnemyPatrol patrolModule;

    private Vector3 lastKnownPosition;
    private bool isSearching;
    private bool isPatrolling;
    private float patrolTimer;

    // ========================================================================
    // INICIALIZAÇÃO (A "Mágica" acontece aqui)
    // ========================================================================
    private void Awake()
    {
        // 1. Pega as referências dos scripts vizinhos
        agent = GetComponent<NavMeshAgent>();
        visionModule = GetComponent<EnemyVision>();
        gunModule = GetComponent<EnemyGun>();
        patrolModule = GetComponent<EnemyPatrol>();

        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();

        // 2. INJETA os valores do Inspector nos módulos
        // Configurando Visão
        visionModule.radius = viewRadius;
        visionModule.obstructionMask = obstacleMask;
        visionModule.targetMask = targetMask;

        // Configurando Arma
        gunModule.spawnBullet = firePoint;
        gunModule.bulletPrefab = bulletPrefab;
        gunModule.radiusBullet = attackRange;
        gunModule.intervalBullet = fireRate;
        gunModule.precisionBullet = accuracy;
        gunModule.damageBullet = damage;
        gunModule.speedBullet = bulletSpeed;
        gunModule.sizeBullet = bulletSize;

        // Configurando Patrulha
        patrolModule.patrolRadius = wanderRadius;
        patrolModule.waitTimeAtPoint = waitTime;
    }

    // ========================================================================
    // LÓGICA (CÉREBRO)
    // ========================================================================
    private void Update()
    {
        // Lê o estado do módulo de visão
        if (visionModule.canSeePlayer)
        {
            EngageTarget();
            UpdateColor(matChase);
        }
        else if (lastKnownPosition != Vector3.zero)
        {
            Investigate();
            UpdateColor(matInvestigate);
        }
        else
        {
            Patrol();
            UpdateColor(matPatrol);
        }
    }

    private void EngageTarget()
    {
        isSearching = false;
        isPatrolling = false;

        lastKnownPosition = visionModule.playerTarget.position;
        agent.isStopped = false;
        agent.SetDestination(visionModule.playerTarget.position);

        // Manda o módulo de arma tentar atirar
        gunModule.TryShoot(visionModule.playerTarget);
    }

    private void Investigate()
    {
        if (!isSearching)
        {
            isSearching = true;
            isPatrolling = false;
            agent.isStopped = false;
            agent.SetDestination(lastKnownPosition);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            lastKnownPosition = Vector3.zero;
            isSearching = false;
        }
    }

    private void Patrol()
    {
        if (isPatrolling)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolModule.waitTimeAtPoint)
                {
                    isPatrolling = false;
                    patrolTimer = 0f;
                }
            }
        }
        else
        {
            Vector3 point;
            if (patrolModule.GetRandomPoint(transform.position, out point))
            {
                agent.SetDestination(point);
                isPatrolling = true;
                agent.isStopped = false;
            }
        }
    }

    private void UpdateColor(Material mat)
    {
        if (meshRenderer != null && mat != null && meshRenderer.sharedMaterial != mat)
            meshRenderer.material = mat;
    }

    // Validação para garantir que não existem erros lógicos
    private void OnValidate()
    {
        if (attackRange > viewRadius) attackRange = viewRadius;
    }

    // ========================================================================
    // FERRAMENTAS VISUAIS DE EDITOR (GIZMOS)
    // ========================================================================
    private void OnDrawGizmosSelected()
    {
        // Pega a posição central do inimigo
        Vector3 center = transform.position;

        // --- GIZMO DE VISÃO (AMARELO) ---
        // Define uma cor amarela semi-transparente (Alpha = 0.3f)
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f);
        // Desenha a esfera preenchida
        Gizmos.DrawSphere(center, viewRadius);
        // Define amarelo sólido para a borda
        Gizmos.color = Color.yellow;
        // Desenha a linha da borda
        Gizmos.DrawWireSphere(center, viewRadius);

        // --- GIZMO DE ATAQUE (VERMELHO) ---
        // Define uma cor vermelha mais forte e semi-transparente (Alpha = 0.5f)
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(center, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);

        // --- GIZMO DE PATRULHA (AZUL CLARO - BÔNUS) ---
        // Útil para ver onde ele pode andar aleatoriamente
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
        Gizmos.DrawSphere(center, wanderRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Enemy script = (Enemy)target;

            // --- 1. LEGENDA DE CORES (NOVO) ---
            // Cria uma caixa visual explicativa no topo
            string legenda = "LEGENDA DE ESTADOS:\n" +
                             "Patrulha (Wander) -> Use cor calma (Verde/Azul)\n" +
                             "Investigação (Search) -> Use cor de alerta (Amarelo/Laranja)\n" +
                             "Perseguição (Chase) -> Use cor de perigo (Vermelho)";

            EditorGUILayout.HelpBox(legenda, MessageType.None);

            // Espaço para separar a legenda dos campos
            EditorGUILayout.Space(10);

            // --- 2. DESENHA O INSPECTOR PADRÃO ---
            // Isso desenha todas as variáveis (SerializeFields) que configuramos lá em cima
            DrawDefaultInspector();

            // --- 3. AVISOS DE ERRO E VALIDAÇÃO ---
            EditorGUILayout.Space();

            // Aviso de Lógica
            if (script.viewRadius < script.attackRange)
            {
                EditorGUILayout.HelpBox("ERRO LÓGICO: O Raio de Ataque (Attack Range) é maior que a Visão! O inimigo vai tentar atirar sem ver.", MessageType.Error);
            }

            // Aviso de Componente Faltando
            if (script.bulletPrefab == null)
            {
                EditorGUILayout.HelpBox("ATENÇÃO: Faltando 'Bullet Prefab'. O inimigo não vai conseguir atirar.", MessageType.Error);
            }

            // Aviso de Referência de Tiro
            if (script.firePoint == null)
            {
                EditorGUILayout.HelpBox("ATENÇÃO: Faltando 'Fire Point'. A bala vai sair dos pés do inimigo!", MessageType.Warning);
            }
        }
    }
#endif
} // Fim da classe Enemy