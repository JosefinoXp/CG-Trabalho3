using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Levitate : MonoBehaviour
{
    [Header("Botão Pulo")]
    [SerializeField]
    public InputActionReference jumpButton = null;

    [Header("CharacterController")]
    [SerializeField]
    public CharacterController charController;

    [Header("Física")]
    public float jumpHeight = 5f;
    private float gravityValue = -9.81f;

    [Header("Plano de Levitação")]
    public GameObject plane;
    public float planeOffset = 0.2f;

    [Header("Timer Levitação")]
    public float levitateDuration = 3.0f; // Tempo máximo (configurável no Inspector)
    [SerializeField] // Apenas para você ver o tempo descendo no Inspector (debug)
    private float currentLevitateTime;

    private Vector3 playerVelocity;
    public bool jumpButtonReleased;
    public bool isTouchingGround;

    private bool isLevitating = false;

    void Start()
    {
        jumpButtonReleased = true;
        currentLevitateTime = levitateDuration; // Começa com tanque cheio

        if (plane != null) plane.SetActive(false);
    }

    void Update()
    {
        float jumpVal = jumpButton.action.ReadValue<float>();

        // 1. INPUT
        if (jumpVal > 0 && jumpButtonReleased == true)
        {
            jumpButtonReleased = false;

            if (isLevitating)
            {
                PararLevitacao();
            }
            else if (charController.isGrounded)
            {
                Jump();
            }
            else
            {
                // Só permite levitar se ainda tiver tempo sobrando
                if (currentLevitateTime > 0)
                {
                    Levitar();
                }
            }
        }
        else if (jumpVal == 0)
        {
            jumpButtonReleased = true;
        }

        // 2. LÓGICA DO TIMER
        if (isLevitating)
        {
            // Diminui o tempo
            currentLevitateTime -= Time.deltaTime;

            // Se o tempo acabar, corta a levitação
            if (currentLevitateTime <= 0)
            {
                PararLevitacao();
            }
        }

        // 3. FÍSICA E RECARGA
        if (charController.isGrounded)
        {
            // Se estiver no chão (real ou plano), zera a queda
            if (playerVelocity.y < 0) playerVelocity.y = -2f;
            isTouchingGround = true;

            // IMPORTANTE: Recarrega o timer apenas se estiver no chão REAL
            // Como sabemos se é chão real? Se NÃO estiver levitando.
            // Se estiver levitando, o isGrounded é true (por causa do plano), mas não queremos recarregar.
            if (!isLevitating)
            {
                currentLevitateTime = levitateDuration;
            }
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
            isTouchingGround = false;
        }

        charController.Move(playerVelocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (isLevitating && plane != null)
        {
            Vector3 novaPosicao = transform.position;
            novaPosicao.y = plane.transform.position.y;
            plane.transform.position = novaPosicao;
        }
    }

    public void Jump()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public void Levitar()
    {
        if (plane == null) return;

        isLevitating = true;
        playerVelocity.y = 0;

        Vector3 spawnPos = transform.position;
        spawnPos.y = transform.position.y - planeOffset;

        plane.transform.position = spawnPos;
        plane.SetActive(true);
    }

    public void PararLevitacao()
    {
        if (plane == null) return;

        isLevitating = false;
        plane.SetActive(false);
    }
}