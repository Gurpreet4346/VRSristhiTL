using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerScript : MonoBehaviour
{
    IAPlayer PlayerInputAction;
    Vector2 movementInput;
    Vector3 MovementDirection;
    Vector3 MovementVelocity;

    CharacterController Cc;
    [SerializeField] float Speed=50;
    [SerializeField] float gravityValue = -9.82f;
    [SerializeField] float jumpHeight = 5f;

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
        Debug.Log(MovementVelocity.y);

    }

    void HandleMovement() {
        MovementDirection = (new Vector3(movementInput.y, 0, movementInput.x)).normalized;
        MovementVelocity = MovementDirection * Speed * Time.deltaTime;
        if (Cc.isGrounded) { MovementVelocity.y = 0; }
        MovementVelocity.y += gravityValue * Time.deltaTime;
        Cc.Move(MovementVelocity);
    }

    void HandleJump() {
        if (Cc.isGrounded) {
            MovementVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            Debug.Log("YO YO BROTHER!!");
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
