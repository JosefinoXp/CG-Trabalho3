//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController))]
//public class Levitate : MonoBehaviour
//{
//    [SerializeField] private InputActionReference jumpButton;
//    [SerializeField] private float jumpHeight = 2.0f;
//    [SerializeField] private float gravityValue = -9.81f;

//    private CharacterController characterController;
//    private Vector3 velocity;

//    private void Awake()
//    {
//        characterController = GetComponent<CharacterController>();
//    }

//    private void OnEnable()
//    {
//        jumpButton.action.performed += Jumping;
//    }

//    private void OnDisable()
//    {
//        jumpButton.action.performed -= Jumping;
//    }

//    private void Jumping(InputAction.CallbackContext obj)
//    {
//        if (!characterController.isGrounded) return;
//        velocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
//    }

//    private void Update()
//    {
//        if (characterController.isGrounded && velocity.y < 0)
//        {
//            velocity.y = 0;
//        }

//        velocity.y += gravityValue * Time.deltaTime;
//        characterController.Move(velocity * Time.deltaTime);
//    }


//}

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

    private Vector3 playerVelocity;

    public bool jumpButtonReleased;

    private bool isTouchingGround;
    // Start is called before the first frame update
    void Start()
    {
        jumpButtonReleased = true;
    }

    // Update is called once per frame
    void Update()
    {

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
        if (charController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            isTouchingGround = true;
        }

        float jumpVal = jumpButton.action.ReadValue<float>();
        if (jumpVal > 0 && jumpButtonReleased == true)
        {
            jumpButtonReleased = false;
            Jump();
            isTouchingGround = false;
        }
        else if (jumpVal == 0)
        {
            jumpButtonReleased = true;
        }
    }

    public void Jump()
    {
        if (isTouchingGround == false)
        {
            return;
        }
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

}