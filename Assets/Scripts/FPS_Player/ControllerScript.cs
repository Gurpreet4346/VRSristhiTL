using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class ControllerScript : MonoBehaviourPunCallbacks
{
    IAPlayer PlayerInputAction;
    Vector2 movementInput;
    Vector3 MovementInput;
    Vector3 MovementVelocity;
    float VerticalVelocity=0;

    int health;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text AmmoClip;
    [SerializeField] TMP_Text AmmoInventory;
    [SerializeField] GameObject UIWeaponImage;
    [SerializeField] GameObject HandAttachPoint;
    [SerializeField] GameObject BackAttachPoint;
    [SerializeField] Camera CameraObj;

    SphereCollider SphereColliderInteraction;

    GameObject GunInteracttable;
    IGun GunHand;
    IGun GunBack;


    CharacterController Cc;
    [SerializeField] float Speed=5;
    float gravityValue = -0.32f;
    float jumpHeight = 0.01f;
    float AppliedGravity;
    [SerializeField] AnimationCurve WalkSprintCurve;
    [SerializeField] float MaxWalkSpeedTime = 1.5f;
    float ACSpeedCounter;
    bool SprintPressed = false;

    Vector3 networkposition;

    [Header("AnimatorSwitchVariables")]
    Animator animator;
    int IsGroundedHash;
    int FwdSpeedHash;
    int RightSpeedHash;



    private void Awake() {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 5;
        Cc = GetComponent<CharacterController>();
        InitializeInputAction();
        SphereColliderInteraction = GetComponent<SphereCollider>();
     //   HashAnimatorParameters();

    }

    private void Update() {
        //   AnimatorParametersUpdate();
        HandleMovement();

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "GunInteractable" && GetComponent<PhotonView>().IsMine) {
            GunInteracttable = other.gameObject;
            if (other.gameObject.TryGetComponent(out IGun GunInterface)) {
                GunInterface.UIGunDisplay(CameraObj);
            }



        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "GunInteractable" && GunInteracttable == other.gameObject) {
            if (other.gameObject.TryGetComponent(out IGun GunInterface)) {
                GunInterface.UIGunUnDisplay();
            }
            GunInteracttable = null;
        }
    }

    void AnimationCurveCounter() {
        if (MovementInput.magnitude > 0) { ACSpeedCounter += Time.deltaTime; } else { ACSpeedCounter = 0; }
        if (!SprintPressed) { if (ACSpeedCounter> MaxWalkSpeedTime) { ACSpeedCounter-=2*Time.deltaTime; } }
    }


    void HandleMovement() {
        if (GetComponent<PhotonView>().IsMine) { 
        MovementInput = (transform.forward*movementInput.y+transform.right*movementInput.x).normalized;
        AnimationCurveCounter();
        MovementVelocity = MovementInput * Speed* WalkSprintCurve.Evaluate(ACSpeedCounter) * Time.deltaTime;
        }

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
        if (GetComponent<PhotonView>().IsMine) { 
        PlayerInputAction.Locomotion.Movement.started += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Movement.performed += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Movement.canceled += ctx => { movementInput = ctx.ReadValue<Vector2>(); };
        PlayerInputAction.Locomotion.Jump.started += ctx => HandleJump();
        PlayerInputAction.Locomotion.Sprint.started += ctx => SprintPressed = true;
        PlayerInputAction.Locomotion.Sprint.canceled += ctx => SprintPressed = false;
        }
    }

    private void OnEnable() {
        base.OnEnable();
        if (GetComponent<PhotonView>().IsMine) {
            PlayerInputAction.Enable();
        } else { PlayerInputAction.Disable(); }
    }
    private void OnDisable() {
        base.OnDisable();
        PlayerInputAction.Disable();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(MovementVelocity);

        } else {
            networkposition = (Vector3) stream.ReceiveNext();
            MovementVelocity = (Vector3) stream.ReceiveNext();
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            networkposition += MovementVelocity * lag;
            transform.position = networkposition;   
        }
    }
}
