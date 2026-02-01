//A DIFERENÇA DESSE SCRIPT PARA BODYSOCKETINVETORY É QUE
//ELE É APENAS UM SOCKET SÓ, ENQUANTO O OUTRO CABE VÁRIOS
//ELE FOI FEITO COM IDEIAL PARA O COLDRE DO JOGADOR FULL BODY

using System.Collections; // Necessário para a Corrotina de tempo
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketPhysicsToggler : MonoBehaviour
{
    [Header("Configurações de Retorno")]
    [Tooltip("Tempo em segundos antes da arma voltar se cair no chão")]
    public float returnDelay = 0.5f;

    private XRSocketInteractor _socket;
    private XRGrabInteractable _assignedItem; // A arma "dona" deste coldre

    void Awake()
    {
        _socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        _socket.selectEntered.AddListener(OnItemPlaced);
        _socket.selectExited.AddListener(OnItemRemovedFromSocket);
    }

    void OnDisable()
    {
        _socket.selectEntered.RemoveListener(OnItemPlaced);
        _socket.selectExited.RemoveListener(OnItemRemovedFromSocket);

        // Limpa eventos da arma se o script for desligado
        if (_assignedItem != null)
        {
            _assignedItem.selectExited.RemoveListener(OnItemDroppedByPlayer);
        }
    }

    // --- LÓGICA 1: QUANDO ENTRA NO SOCKET ---
    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        // 1. Parte Física (Original)
        SetCollidersToTrigger(args.interactableObject.transform, true);

        // 2. Parte de Retorno (Nova)
        XRGrabInteractable newItem = args.interactableObject as XRGrabInteractable;

        if (newItem != null)
        {
            // Se trocou de arma, para de vigiar a antiga
            if (_assignedItem != null && _assignedItem != newItem)
            {
                _assignedItem.selectExited.RemoveListener(OnItemDroppedByPlayer);
            }

            // Define a nova dona e começa a vigiar se o jogador vai soltá-la
            _assignedItem = newItem;
            _assignedItem.selectExited.RemoveListener(OnItemDroppedByPlayer); // Evita duplicar
            _assignedItem.selectExited.AddListener(OnItemDroppedByPlayer);
        }
    }

    // --- LÓGICA 2: QUANDO SAI DO SOCKET (PEGOU COM A MÃO) ---
    private void OnItemRemovedFromSocket(SelectExitEventArgs args)
    {
        // 1. Parte Física (Original) - Volta a ser sólida
        SetCollidersToTrigger(args.interactableObject.transform, false);

        // Nota: NÃO limpamos o _assignedItem aqui. 
        // O coldre continua "dono" da arma mesmo enquanto você a segura.
    }

    // --- LÓGICA 3: QUANDO O JOGADOR SOLTA A ARMA (NO CHÃO OU NO AR) ---
    private void OnItemDroppedByPlayer(SelectExitEventArgs args)
    {
        // Verifica se o item solto é realmente a arma vinculada a este coldre
        if (_assignedItem != null && args.interactableObject == _assignedItem)
        {
            StartCoroutine(ReturnRoutine());
        }
    }

    private IEnumerator ReturnRoutine()
    {
        // Espera um pouco (dá tempo de pegar com a outra mão ou cair no chão)
        yield return new WaitForSeconds(returnDelay);

        // CHECAGENS DE SEGURANÇA:
        // 1. A arma ainda existe?
        // 2. O Socket está vazio? (!hasSelection)
        // 3. A arma NÃO está na mão do jogador? (!isSelected) -> Isso é importante pra não roubar da mão
        if (_assignedItem != null && !_socket.hasSelection && !_assignedItem.isSelected)
        {
            // Zera a física para ela não voar loucamente
            if (_assignedItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Força o socket a puxar a arma de volta
            _socket.StartManualInteraction(_assignedItem as IXRSelectInteractable);
        }
    }

    // --- MÉTODOS AUXILIARES (ORIGINAIS) ---

    private void SetCollidersToTrigger(Transform itemRoot, bool isTriggerState)
    {
        Collider[] colliders = itemRoot.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.isTrigger = isTriggerState;
        }
    }
}