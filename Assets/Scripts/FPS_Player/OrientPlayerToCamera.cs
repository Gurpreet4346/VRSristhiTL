using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
public class OrientPlayerToCamera : MonoBehaviour
{
    [SerializeField] PhotonView PhotonView;
    [SerializeField] Transform PlayerTransform;
    private float yaw;
    private float pitch=0;
    [SerializeField] private float senstivity=10;
    IAPlayer InputActionPlayer;
    Vector2 LookInput;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        InputActionPlayer = new IAPlayer();
    }

    void Update()
    {
        if (PhotonView.IsMine) {
            CalculateInput();
            PlayerTransform.Rotate(Vector3.up * yaw);
            transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void CalculateInput()
    {
        LookInput = InputActionPlayer.Locomotion.Look.ReadValue<Vector2>();
        yaw = LookInput.x* senstivity*Time.deltaTime;
        pitch = Mathf.Clamp(pitch - LookInput.y * senstivity * Time.deltaTime, -85f, 85f);

    }
    private void OnEnable() {
        if (PhotonView.IsMine) { 
        InputActionPlayer.Locomotion.Enable();
        }
    }
    private void OnDisable() {
        InputActionPlayer.Locomotion.Disable();
    }


}
