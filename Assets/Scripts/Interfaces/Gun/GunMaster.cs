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
    bool FirePressed;
    bool firingCooldownActive;
    Vector3 destination;
    [SerializeField] Transform GunTip;
    BulletMaster projectile;

    //break condition => shoot.cancelled => StopCoroutine

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


    IEnumerator FireOnServer(Camera cam) {
        if (!firingCooldownActive && !IsReloading) {
            if (ClipAmmo > 0) {
                ShootProjectile(cam);
                firingCooldownActive = true;

            } else {
                StartCoroutine(Reload());
                yield break;
            }
            yield return new WaitForSeconds(FiringRateBulletShootInterval);
            firingCooldownActive = false;
            if (isAutomatic) {
                yield return StartCoroutine(FireOnServer(cam));
            } else { yield break; }
        } else { yield break; }


    }



    void ShootProjectile(Camera cam) {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
