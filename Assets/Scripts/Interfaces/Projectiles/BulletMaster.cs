using UnityEngine;

public class BulletMaster : MonoBehaviour
{
    Rigidbody rb;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void FireProjectile(Vector3 destination, Vector3 FireTip, float projectilespeed) {
        rb.linearVelocity=(destination - FireTip).normalized * projectilespeed;
    }
}
