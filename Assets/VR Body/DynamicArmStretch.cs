using UnityEngine;
using UnityEngine.Animations.Rigging;

// Este script deve ser colocado no mesmo objeto que tem o TwoBoneIKConstraint do braço
public class DynamicArmStretch : MonoBehaviour
{
    [Header("Configurações do IK")]
    public TwoBoneIKConstraint armIK; // Arraste o componente IK do braço aqui

    [Header("Ossos (Preencher Manualmente)")]
    public Transform upperArmBone; // O osso do braço (bíceps)
    public Transform foreArmBone;  // O osso do antebraço
    // NÃO coloque o osso da mão aqui

    [Header("Ajustes")]
    [Tooltip("O eixo que aponta para o próximo osso. No Mixamo geralmente é o Y.")]
    public Vector3 stretchAxis = new Vector3(0, 1, 0);
    [Range(1.0f, 1.5f)]
    [Tooltip("O máximo que o braço pode esticar (1.2 = 20% a mais).")]
    public float maxStretchMultiplier = 1.2f;

    private float _initialArmLength;
    private Vector3 _initialUpperScale;
    private Vector3 _initialForeScale;

    void Start()
    {
        // Salva as escalas originais para podermos voltar a elas depois
        _initialUpperScale = upperArmBone.localScale;
        _initialForeScale = foreArmBone.localScale;

        // Calcula o comprimento total do braço em repouso
        // (Distância do ombro ao cotovelo + distância do cotovelo à mão)
        float upperLen = Vector3.Distance(upperArmBone.position, foreArmBone.position);
        float foreLen = Vector3.Distance(foreArmBone.position, armIK.data.tip.position);
        _initialArmLength = upperLen + foreLen;
    }

    // Usamos LateUpdate para agir DEPOIS que o IK já tentou posicionar o braço
    void LateUpdate()
    {
        if (armIK == null || armIK.weight == 0) return;

        // Onde o braço começa (ombro)
        Vector3 rootPos = armIK.data.root.position;
        // Onde queremos chegar (o controle VR)
        Vector3 targetPos = armIK.data.target.position;

        // Qual a distância necessária agora?
        float currentNeededDistance = Vector3.Distance(rootPos, targetPos);

        // Se a distância necessária for maior que o braço...
        if (currentNeededDistance > _initialArmLength)
        {
            // Calcula o fator de estiramento
            float stretchFactor = currentNeededDistance / _initialArmLength;

            // Limita o estiramento ao máximo permitido para não virar um monstro
            stretchFactor = Mathf.Clamp(stretchFactor, 1.0f, maxStretchMultiplier);

            // Cria o vetor de escala baseado no eixo escolhido (geralmente Y)
            Vector3 targetScale = Vector3.one + (stretchAxis * (stretchFactor - 1.0f));

            // Aplica a escala NOS OSSOS (sem afetar a mão, pois ela é filha do antebraço mas o IK segura a posição dela)
            // Nota: Dependendo de como seu rig está feito, a mão pode crescer junto. 
            // Se isso acontecer, precisaremos de um script para contra-escalar a mão.
            upperArmBone.localScale = Vector3.Scale(_initialUpperScale, targetScale);
            foreArmBone.localScale = Vector3.Scale(_initialForeScale, targetScale);
        }
        else
        {
            // Se o braço alcança, volta para a escala original
            upperArmBone.localScale = _initialUpperScale;
            foreArmBone.localScale = _initialForeScale;
        }
    }
}