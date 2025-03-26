using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerScript : MonoBehaviour
{
    IAPlayer PlayerInputAction;
    Vector2 movementInput;
    Vector3 MovementInput;
    Vector3 MovementVelocity;
    float VerticalVelocity=0;

    CharacterController Cc;
    [SerializeField] float Speed=5;
    float gravityValue = -0.32f;
    float jumpHeight = 0.01f;
    float AppliedGravity;
    [SerializeField] AnimationCurve WalkSprintCurve;
    [SerializeField] float MaxWalkSpeedTime = 1.5f;
    float ACSpeedCounter;
    bool SprintPressed = false;

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

    void AnimationCurveCounter() {
        if (MovementInput.magnitude > 0) { ACSpeedCounter += Time.deltaTime; } else { ACSpeedCounter = 0; }
        if (!SprintPressed) { if (ACSpeedCounter> MaxWalkSpeedTime) { ACSpeedCounter-=2*Time.deltaTime; } }
    }

    void HandleMovement() {
        MovementInput = (transform.forward*movementInput.y+transform.right*movementInput.x).normalized;
        AnimationCurveCounter();
        MovementVelocity = MovementInput * Speed* WalkSprintCurve.Evaluate(ACSpeedCounter) * Time.deltaTime;
        if (Cc.isGrounded && VerticalVelocity < 0) { 
            VerticalVelocity = 0; 
            AppliedGravity = gravityValue; // Small Gravity Value when grounded 
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
        PlayerInputAction.Locomotion.Sprint.started += ctx => SprintPressed = true;
        PlayerInputAction.Locomotion.Sprint.canceled += ctx => SprintPressed = false;

    }

    private void OnEnable() {
        PlayerInputAction.Enable(); 
    }
    private void OnDisable() {
        PlayerInputAction.Disable();
    }
}
