//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class bodySocket
//{
//    public GameObject gameObject;
//    [Range(0.01f, 1f)]
//    public float heightRatio;
//}

//public class BodySocketInventory : MonoBehaviour

//{
//    public GameObject HMD;
//    public bodySocket[] bodySockets;

//    private Vector3 _currentHMDlocalPosition;
//    private Quaternion _currentHMDRotation;
//    void Update()
//    {
//        _currentHMDlocalPosition = HMD.transform.localPosition;
//        _currentHMDRotation = HMD.transform.rotation;
//        foreach (var bodySocket in bodySockets)
//        {
//            UpdateBodySocketHeight(bodySocket);
//        }
//        UpdateSocketInventory();
//    }

//    private void UpdateBodySocketHeight(bodySocket bodySocket)
//    {

//        bodySocket.gameObject.transform.localPosition = new Vector3(bodySocket.gameObject.transform.localPosition.x, (_currentHMDlocalPosition.y * bodySocket.heightRatio), bodySocket.gameObject.transform.localPosition.z);
//    }

//    private void UpdateSocketInventory()
//    {
//        transform.localPosition = new Vector3(_currentHMDlocalPosition.x, 0, _currentHMDlocalPosition.z);
//        transform.rotation = new Quaternion(transform.rotation.x, _currentHMDRotation.y, transform.rotation.z, _currentHMDRotation.w);
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class BodySocketData
{
    public string socketName;
    public GameObject socketObject;

    [Range(0.01f, 1f)]
    public float heightRatio;

    // O item agora é privado na Inspector, pois será definido no jogo
    // Mas deixamos público para debug se precisar ver quem está lá
    [HideInInspector] public XRGrabInteractable assignedItem;

    // Cache interno do componente Socket
    [HideInInspector] public XRSocketInteractor cachedInteractor;
}

public class BodySocketInventory : MonoBehaviour
{
    [Header("Setup Geral")]
    public GameObject HMD;
    public float returnDelay = 0.5f;

    [Header("Configuração dos Sockets")]
    public BodySocketData[] bodySockets;

    private Vector3 _currentHMDlocalPosition;
    private Quaternion _currentHMDRotation;

    void Start()
    {
        foreach (var data in bodySockets)
        {
            if (data.socketObject != null)
            {
                // 1. Pega o componente XRSocketInteractor
                data.cachedInteractor = data.socketObject.GetComponent<XRSocketInteractor>();

                if (data.cachedInteractor != null)
                {
                    // 2. Inscreve no evento "SelectEntered" (Quando ALGO entra no socket)
                    // Isso é o que faz o vínculo dinâmico
                    data.cachedInteractor.selectEntered.AddListener((args) => OnItemPlacedInSocket(data, args));
                }
            }
        }
    }

    void OnDisable()
    {
        // Limpeza de eventos para evitar erros de memória
        foreach (var data in bodySockets)
        {
            if (data.assignedItem != null)
            {
                data.assignedItem.selectExited.RemoveListener((args) => OnItemDropped(data));
            }

            if (data.cachedInteractor != null)
            {
                data.cachedInteractor.selectEntered.RemoveAllListeners();
            }
        }
    }

    void Update()
    {
        // --- Lógica de Posicionamento (Mantida igual) ---
        _currentHMDlocalPosition = HMD.transform.localPosition;
        _currentHMDRotation = HMD.transform.rotation;

        foreach (var data in bodySockets)
        {
            if (data.socketObject != null)
                UpdateBodySocketHeight(data);
        }

        UpdateSocketInventory();
    }

    private void UpdateBodySocketHeight(BodySocketData data)
    {
        data.socketObject.transform.localPosition = new Vector3(
            data.socketObject.transform.localPosition.x,
            (_currentHMDlocalPosition.y * data.heightRatio),
            data.socketObject.transform.localPosition.z
        );
    }

    private void UpdateSocketInventory()
    {
        transform.localPosition = new Vector3(_currentHMDlocalPosition.x, 0, _currentHMDlocalPosition.z);
        transform.rotation = new Quaternion(transform.rotation.x, _currentHMDRotation.y, transform.rotation.z, _currentHMDRotation.w);
    }

    // --- NOVA LÓGICA DE VÍNCULO DINÂMICO ---

    // Chamado automaticamente quando QUALQUER coisa é colocada em um socket
    private void OnItemPlacedInSocket(BodySocketData socketData, SelectEnterEventArgs args)
    {
        // O objeto que entrou no socket
        XRGrabInteractable newItem = args.interactableObject as XRGrabInteractable;

        if (newItem == null) return;

        // Se este socket JÁ tinha esse item registrado, não faz nada
        if (socketData.assignedItem == newItem) return;

        // --- SEGURANÇA: GARANTIR QUE O ITEM SÓ TENHA UMA "CASA" ---
        // Se eu tirei a arma da Esquerda e coloquei na Direita, 
        // a Esquerda precisa "esquecer" a arma.
        foreach (var otherSocket in bodySockets)
        {
            if (otherSocket.assignedItem == newItem)
            {
                // Remove o listener do socket antigo
                otherSocket.assignedItem.selectExited.RemoveListener((evt) => OnItemDropped(otherSocket));
                otherSocket.assignedItem = null; // O socket antigo agora está "livre"
            }
        }

        // --- REGISTRA A NOVA CASA ---

        // 1. Limpa listeners antigos desse item (caso existam)
        newItem.selectExited.RemoveAllListeners();

        // 2. Define o novo item deste socket
        socketData.assignedItem = newItem;

        // 3. Adiciona o listener: Se o jogador soltar ESSE item, chama a função de retorno DESTE socket
        newItem.selectExited.AddListener((evt) => OnItemDropped(socketData));

        Debug.Log($"Item {newItem.name} vinculado ao socket {socketData.socketName}");
    }

    // Chamado quando o jogador solta a arma (cai no chão)
    private void OnItemDropped(BodySocketData data)
    {
        // Só retorna se o item solto ainda for o item dono deste socket
        if (data.assignedItem != null)
        {
            StartCoroutine(ReturnRoutine(data));
        }
    }

    private IEnumerator ReturnRoutine(BodySocketData data)
    {
        yield return new WaitForSeconds(returnDelay);

        // Verificações de segurança:
        if (data.assignedItem == null) yield break;
        if (data.assignedItem.isSelected) yield break; // Está na mão
        if (data.cachedInteractor.hasSelection) yield break; // Socket ocupado

        // Retorna o item
        Rigidbody itemRb = data.assignedItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.velocity = Vector3.zero;
            itemRb.angularVelocity = Vector3.zero;
        }

        data.cachedInteractor.StartManualInteraction(data.assignedItem as IXRSelectInteractable);
    }
}