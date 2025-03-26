using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerScript : MonoBehaviour
{
    IAPlayer PlayerInputAction;
    Vector2 movementInput;
    Vector3 MovementDirection;
    Vector3 MovementVelocity;
    float VerticalVelocity=0;

    CharacterController Cc;
    [SerializeField] float Speed=5;
    float gravityValue = -0.0982f;
    float jumpHeight = 0.05f;
    float AppliedGravity;

    [Header("AnimatorSwitchVariables")]
    Animator animator;
    int IsGroundedHash;
    int FwdSpeedHash;
    int RightSpeedHash;

    private void Awake() {
        Cc = GetComponent<CharacterController>();
        InitializeInputAction();
     //   HashAnimatorParameters();

    }

    private void Update() {
        //   AnimatorParametersUpdate();
        HandleMovement();

    }

    void HandleMovement() {
        MovementDirection = (transform.forward*movementInput.y+transform.right*movementInput.x).normalized;
        MovementVelocity = MovementDirection * Speed * Time.deltaTime;
        if (Cc.isGrounded && VerticalVelocity < 0) { 
            VerticalVelocity = 0; 
            AppliedGravity = 0.01f; // Small Gravity Value when grounded 
        }  else { AppliedGravity = gravityValue; }
        VerticalVelocity += AppliedGravity * Time.deltaTime;
        Debug.Log(VerticalVelocity);

        Cc.Move(new Vector3(MovementVelocity.x, VerticalVelocity, MovementVelocity.z));

    }

    void HandleJump() {
        if (Cc.isGrounded) {
            VerticalVelocity += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }

    void AnimatorParametersUpdate() {
        if (!Cc.isGrounded) { animator.SetBool(IsGroundedHash, false); } else { animator.SetBool(IsGroundedHash, true); }
        animator.SetFloat(RightSpeedHash, movementInput.x);
        animator.SetFloat(FwdSpeedHash, movementInput.y);
    }

    void HashAnimatorParameters() {
        animator = GetComponent<Animator>();
        IsGroundedHash = Animator.StringToHash("IsGrounded");
        FwdSpeedHash = Animator.StringToHash("ForwardSpeed");
        RightSpeedHash = Animator.StringToHash("RightSpeed");
    }

    void InitializeInputAction() {
        PlayerInputAction = new IAPlayer();
        PlayerInputAction.Locomotion.Movement.started += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Movement.performed += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Movement.canceled += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Jump.started += ctx => HandleJump();
    }

    private void OnEnable() {
        PlayerInputAction.Enable(); 
    }
    private void OnDisable() {
        PlayerInputAction.Disable();
    }
}
