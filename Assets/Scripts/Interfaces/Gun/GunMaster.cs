using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
public class GunMaster : MonoBehaviour, IGun {
    bool interactable = true;
    [SerializeField] int InventoryAmmo=30;
    [SerializeField] int MaxClipSize=30;
    [SerializeField] int ProjectileSpeed= 500;
    [SerializeField] int ClipAmmo = 30;
    [SerializeField] float FiringRateBulletShootInterval=0.1f;
    [SerializeField] float reloadTime=3f;
    [SerializeField] bool isAutomatic;
    [Header("UI")]
    TMP_Text ClipAmmoAmount;
    TMP_Text InventoryAmmoAmount;
    GameObject GunImageUI;
    [SerializeField] GameObject UICanvas;
    [SerializeField] Sprite GunImage;

    Vector3 lookDirection;

    GameObject Player;
    private SphereCollider SphereCollider;
    private BoxCollider BoxCollider;
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
    Camera cam;
    bool AI;
    bool UILookAtPlayer;

    bool InHand =false;
    bool pickedup = false;

    //break condition => shoot.cancelled => StopCoroutine

    private void Awake() {
        GunRigidBody= GetComponent<Rigidbody>();
        SphereCollider=GetComponent<SphereCollider>();
        BoxCollider=GetComponent<BoxCollider>();
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

        if (UILookAtPlayer) {
            lookDirection = cam.transform.position - UICanvas.transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                UICanvas.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }

        }
    }

    public void UIGunDisplay(Camera p_cam) {
        cam = p_cam;
        UICanvas.SetActive(true);
        UILookAtPlayer = true;
        
    }
    public void UIGunUnDisplay() {
        cam = null;
        UICanvas.SetActive(false);
        UILookAtPlayer = false;

    }



    public void HandShake(GameObject p_HandAttachPoint,GameObject p_BackAttachPoint,  bool p_AI, TMP_Text AmmoClip, TMP_Text AmmoInventory, GameObject UIWeaponImage) {
        HandAttachPoint = p_HandAttachPoint;
        BackAttachPoint = p_BackAttachPoint;
        AI = p_AI;
        UILookAtPlayer = false;
        interactable = false;
        SphereCollider.enabled = false;
        BoxCollider.enabled = false;
        ClipAmmoAmount = AmmoClip;
        InventoryAmmoAmount = AmmoInventory;
        GunImageUI = UIWeaponImage;
        UIWeaponImage.GetComponent<Image>().sprite= GunImage;
        ClipAmmoAmount.text = ClipAmmo.ToString();
        InventoryAmmoAmount.text = InventoryAmmo.ToString();

        UIWeaponImage.SetActive(true);
        AmmoClip.gameObject.SetActive(true);
        AmmoInventory.gameObject.SetActive(true);

        EquipWeaponInHand();
        UICanvas.SetActive(false);

    }

    public void EquipWeaponInHand() {
        InHand= true;
        pickedup = true;
        GunRigidBody.isKinematic = true; 
    }

    public void StoreWeaponOnBack() {
        InHand = false;
        pickedup = true;
        GunRigidBody.isKinematic = true;
    }

    public void DropWeapon() {
        GunImageUI.SetActive(false);
        ClipAmmoAmount.gameObject.SetActive(false);
        InventoryAmmoAmount.gameObject.SetActive(false);
        GunImageUI=null;
        ClipAmmoAmount = null;
        InventoryAmmoAmount = null;
        pickedup = false;
        InHand = false;
        GunRigidBody.isKinematic = false;
        HandAttachPoint = null;
        BackAttachPoint= null;
        interactable = true;
        SphereCollider.enabled = true;
        BoxCollider.enabled = true;

    }


    public void GunTriggerPressed() {
        FirePressed = true;
        StartCoroutine(FireOnServer());
    }

    public void GunTriggerRelease() {
        FirePressed = false;
    }

    public void ReloadGun() {
        StartCoroutine(Reload());
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
            ClipAmmoAmount.text = ClipAmmo.ToString();
            InventoryAmmoAmount.text = InventoryAmmo.ToString();

            IsReloading = false;

        }

    }


    IEnumerator FireOnServer() {
        if (!firingCooldownActive && !IsReloading) {
            if (ClipAmmo > 0) {
                ShootProjectile();
                firingCooldownActive = true;
                ClipAmmo--;
                ClipAmmoAmount.text = ClipAmmo.ToString();

            } else {
                StartCoroutine(Reload());
                yield break;
            }
            yield return new WaitForSeconds(FiringRateBulletShootInterval);
            firingCooldownActive = false;
            if (isAutomatic && FirePressed) {
                yield return StartCoroutine(FireOnServer());
            } else { yield break; }
        } else { yield break; }


    }



    void ShootProjectile() {
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
