using System.Collections;
using UnityEngine;

public class GunMaster : MonoBehaviour
{
    [SerializeField] int InventoryAmmo=30;
    [SerializeField] int MaxClipSize=30;
    [SerializeField] int ProjectileSpeed= 500;
    [SerializeField] int ClipAmmo = 30;
    [SerializeField] float FiringRateBulletShootInterval=0.1f;
    [SerializeField] float reloadTime=3f;
    [SerializeField] bool isAutomatic;
    bool IsReloading;
    bool FirePressed=true;
    bool firingCooldownActive;
    Vector3 destination;
    [SerializeField] Transform GunTip;
    BulletMaster projectile;
    Ray ray;
    GameObject HandAttachPoint;
    GameObject BackAttachPoint;
    Rigidbody GunRigidBody;

    bool InHand =false;
    bool pickedup = false;

    //break condition => shoot.cancelled => StopCoroutine

    private void Awake() {
        GunRigidBody= GetComponent<Rigidbody>();
    }

    private void Update() {
        if (InHand) { 
            transform.position = HandAttachPoint.transform.position;
            transform.rotation = HandAttachPoint.transform.rotation;
            transform.localScale = HandAttachPoint.transform.localScale;
        }
        if (!InHand && pickedup) {
            transform.position = BackAttachPoint.transform.position;
            transform.rotation = BackAttachPoint.transform.rotation;
            transform.localScale = BackAttachPoint.transform.localScale;
        }
    }

    public void EquipWeaponInHand(GameObject AttachmentPoint) {
        HandAttachPoint= AttachmentPoint;
        InHand= true;
        pickedup = true;
        GunRigidBody.isKinematic = true; 
    }

    public void StoreWeaponOnBack(GameObject AttachmentPoint) {
        BackAttachPoint = AttachmentPoint;
        InHand = false;
        pickedup = true;
        GunRigidBody.isKinematic = true;
    }

    public void DropWeapon() {
        pickedup = false;
        InHand = false;
        GunRigidBody.isKinematic = false;
    }


    public void TriggerPressed(Camera cam, bool AI) {
        FirePressed = true;
        StartCoroutine(FireOnServer(cam, AI));
    }

    public void TriggerRelease() {
        FirePressed = false;
    }


    IEnumerator Reload() {
        IsReloading = true;
        if (ClipAmmo < MaxClipSize && InventoryAmmo > 0) {
            yield return new WaitForSeconds(reloadTime);
            if (InventoryAmmo > MaxClipSize) {
                ClipAmmo = MaxClipSize;
                InventoryAmmo -= MaxClipSize;
            } else {
                ClipAmmo = InventoryAmmo;
                InventoryAmmo = 0;
            }
            IsReloading = false;

        }

    }


    IEnumerator FireOnServer(Camera cam, bool AI) {
        if (!firingCooldownActive && !IsReloading) {
            if (ClipAmmo > 0) {
                ShootProjectile(cam, AI);
                firingCooldownActive = true;
                ClipAmmo--;

            } else {
                StartCoroutine(Reload());
                yield break;
            }
            yield return new WaitForSeconds(FiringRateBulletShootInterval);
            firingCooldownActive = false;
            if (isAutomatic && FirePressed) {
                yield return StartCoroutine(FireOnServer(cam, AI));
            } else { yield break; }
        } else { yield break; }


    }



    void ShootProjectile(Camera cam, bool AI) {
        if (!AI) {
            ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        } else {
            ray = new Ray(GunTip.position, GunTip.forward);
        }

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            destination = hit.point;
        } else {
            destination = ray.GetPoint(1000);
        }
        BulletMaster InstProjectile = Instantiate(projectile, GunTip.position, Quaternion.identity);
        InstProjectile.FireProjectile(destination, GunTip.position,ProjectileSpeed);
    }





}
