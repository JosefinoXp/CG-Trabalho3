using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class HolsterSocketData
{
    public string socketName;
    public GameObject socketObject;

    // Removi o heightRatio pois não vamos mais calcular altura

    // O item que "mora" neste socket
    [HideInInspector] public XRGrabInteractable assignedItem;

    // Cache interno do componente Socket
    [HideInInspector] public XRSocketInteractor cachedInteractor;
}

public class HolsterLogic : MonoBehaviour
{
    [Header("Configuração")]
    public float returnDelay = 0.5f;

    [Header("Lista de Sockets")]
    public HolsterSocketData[] bodySockets;

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
                    // Isso cria o vínculo: "Este item agora pertence a este socket"
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
                // Remove o listener do item se o script for desligado
                data.assignedItem.selectExited.RemoveListener((args) => OnItemDropped(data));
            }

            if (data.cachedInteractor != null)
            {
                data.cachedInteractor.selectEntered.RemoveAllListeners();
            }
        }
    }

    // O Update foi removido completamente pois a posição já está correta no seu projeto.

    // --- LÓGICA DE VÍNCULO DINÂMICO ---

    // Chamado automaticamente quando QUALQUER coisa é colocada em um socket
    private void OnItemPlacedInSocket(HolsterSocketData socketData, SelectEnterEventArgs args)
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

        // 1. Limpa listeners antigos desse item (caso existam de interações passadas)
        // Nota: Isso pode remover sons ou haptics se eles usarem listeners anônimos no SelectExited,
        // mas é necessário para garantir que a lógica de retorno não duplique.
        newItem.selectExited.RemoveAllListeners();

        // 2. Define o novo item deste socket
        socketData.assignedItem = newItem;

        // 3. Adiciona o listener: Se o jogador soltar ESSE item, chama a função de retorno DESTE socket
        newItem.selectExited.AddListener((evt) => OnItemDropped(socketData));

        Debug.Log($"Item {newItem.name} vinculado ao socket {socketData.socketName}");
    }

    // Chamado quando o jogador solta a arma (cai no chão)
    private void OnItemDropped(HolsterSocketData data)
    {
        // Só retorna se o item solto ainda for o item dono deste socket
        if (data.assignedItem != null)
        {
            StartCoroutine(ReturnRoutine(data));
        }
    }

    private IEnumerator ReturnRoutine(HolsterSocketData data)
    {
        yield return new WaitForSeconds(returnDelay);

        // Verificações de segurança:
        if (data.assignedItem == null) yield break; // Item não existe mais
        if (data.assignedItem.isSelected) yield break; // O jogador pegou o item de novo no meio do ar
        if (data.cachedInteractor.hasSelection) yield break; // O socket já está ocupado por outra coisa

        // Para a física do objeto para ele não voar loucamente ao ser puxado
        Rigidbody itemRb = data.assignedItem.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.velocity = Vector3.zero;
            itemRb.angularVelocity = Vector3.zero;
        }

        // Força o socket a interagir manualmente com o objeto, puxando-o de volta
        data.cachedInteractor.StartManualInteraction(data.assignedItem as IXRSelectInteractable);
    }
}