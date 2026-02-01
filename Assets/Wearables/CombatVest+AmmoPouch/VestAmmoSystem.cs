using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VestAmmoSystem : MonoBehaviour
{
    [Header("Configurações")]
    public XRSocketInteractor mainSocket;
    public Transform[] ammoSlots;

    [Header("Debug")]
    public bool showDebugLogs = true;

    // Nomes das Layers (Verifique Project Settings > Interaction Layers)
    private const string LAYER_DEFAULT = "Default";
    private const string LAYER_STORED = "Stored";

    private void OnEnable()
    {
        if (mainSocket != null)
            mainSocket.selectEntered.AddListener(OnMagazineReceived);
    }

    private void OnDisable()
    {
        if (mainSocket != null)
            mainSocket.selectEntered.RemoveListener(OnMagazineReceived);
    }

    private void OnMagazineReceived(SelectEnterEventArgs args)
    {
        XRGrabInteractable magazine = args.interactableObject as XRGrabInteractable;
        if (magazine == null) return;

        Transform freeSlot = GetFirstFreeSlot();

        if (freeSlot != null)
        {
            // Inicia o processo de guardar
            StartCoroutine(TransferAmmoRoutine(magazine, freeSlot));
        }
        else
        {
            // Colete cheio, cospe fora
            mainSocket.interactionManager.SelectExit(mainSocket, magazine);
        }
    }

    private IEnumerator TransferAmmoRoutine(XRGrabInteractable magazine, Transform slot)
    {
        // 1. Solta do Socket Mestre
        mainSocket.interactionManager.SelectExit(mainSocket, magazine);

        // 2. Espera um frame físico
        yield return new WaitForFixedUpdate();

        // 3. Muda a Layer para 'Stored' (Socket Mestre deve ignorar esta layer)
        int storedMask = InteractionLayerMask.GetMask(LAYER_STORED);
        if (storedMask != 0) magazine.interactionLayers = storedMask;

        // 4. CONFIGURAÇÃO DE FÍSICA (Resolve o problema de travar o personagem)
        if (magazine.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false; // Desliga gravidade
            rb.isKinematic = true; // Trava posição
        }

        // SOLUÇÃO PARA O ANDAR: Transforma o colisor em Trigger
        // Assim ele não bate no corpo do jogador
        foreach (Collider col in magazine.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true;
        }

        // 5. Move para o slot e zera posições
        magazine.transform.SetParent(slot);
        magazine.transform.localPosition = Vector3.zero;
        magazine.transform.localRotation = Quaternion.identity;

        Log($"Munição guardada no slot {slot.name}.");

        // Inscreve para o evento de "Pegar" (SelectEnter)
        magazine.selectEntered.AddListener(OnMagazineGrabbedFromVest);
    }

    private Transform GetFirstFreeSlot()
    {
        foreach (Transform slot in ammoSlots)
        {
            if (slot.childCount == 0) return slot;
        }
        return null;
    }

    // Chamado quando a Mão do jogador pega a munição do colete
    private void OnMagazineGrabbedFromVest(SelectEnterEventArgs args)
    {
        XRGrabInteractable magazine = args.interactableObject as XRGrabInteractable;

        // Remove o listener para não rodar isso toda vez que trocar de mão
        magazine.selectEntered.RemoveListener(OnMagazineGrabbedFromVest);

        Log($"Munição retirada. Restaurando física...");

        // 1. Restaura a Layer Default (Para poder interagir com tudo)
        magazine.interactionLayers = InteractionLayerMask.GetMask(LAYER_DEFAULT);

        // 2. Restaura o Colisor Sólido (IsTrigger = false)
        // Isso faz ela voltar a colidir com o chão/paredes
        foreach (Collider col in magazine.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = false;
        }

        // 3. Restaura a Física (Resolve o problema de flutuar)
        if (magazine.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            // Importante: O XRGrabInteractable vai controlar o IsKinematic enquanto segura,
            // mas precisamos garantir que a gravidade volte a funcionar quando soltar.
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        magazine.transform.SetParent(null);
    }

    private void Log(string msg) { if (showDebugLogs) Debug.Log($"[COLETE]: {msg}"); }
}