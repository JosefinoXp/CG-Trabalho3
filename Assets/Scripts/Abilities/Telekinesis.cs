//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

//public class Telekineses : MonoBehaviour
//{
//    [Header("Throw Velocity")]
//    [SerializeField]
//    public float objectSpeed = 5;

//    XRGrabInteractable grabbable;

//    // Start is called before the first frame update
//    void Start()
//    {
//        grabbable = GetComponent<XRGrabInteractable>();
//        grabbable.activated.AddListener(ThrowObject);
//    }

//    public void ThrowObject(ActivateEventArgs arg)
//    {
//        grabbable.enabled = false;
//        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * objectSpeed;
//    }
//}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Telekineses : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float objectSpeed = 10f;

    private XRGrabInteractable grabbable;
    private Rigidbody rb;

    void Awake()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // Inscreve no evento de Activate (Gatilho pressionado)
        grabbable.activated.AddListener(OnActivate);
    }

    void OnDisable()
    {
        // Boa prática: remover listener ao desabilitar
        grabbable.activated.RemoveListener(OnActivate);
    }

    private void OnActivate(ActivateEventArgs args)
    {
        // 1. Identificar quem está segurando (a mão/controle)
        // args.interactorObject é quem ativou o objeto
        var interactor = args.interactorObject;

        // 2. Definir a direção do lançamento
        // Usamos o forward do Interactor (a mão) e não do objeto, 
        // para que o tiro vá para onde o jogador está apontando.
        Vector3 direction = interactor.transform.forward;

        // 3. Forçar o "Drop" (Soltar o objeto)
        // Pedimos ao Manager para desfazer a conexão entre a mão e o objeto
        grabbable.interactionManager.SelectExit(interactor as IXRSelectInteractor, grabbable);

        // 4. Aplicar a física
        // Zeramos a velocidade anterior para um lançamento limpo
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Usamos ForceMode.Impulse para um tiro instantâneo
        rb.AddForce(direction * objectSpeed, ForceMode.Impulse);
    }
}