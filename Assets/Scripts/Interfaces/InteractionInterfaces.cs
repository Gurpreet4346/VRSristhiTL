using TMPro;
using UnityEngine;

public interface IGun {
    void UIGunDisplay(Camera p_cam);
    void UIGunUnDisplay();
    void HandShake(GameObject p_HandAttachPoint, GameObject p_BackAttachPoint,  bool p_AI, TMP_Text AmmoClip, TMP_Text AmmoInventory, GameObject UIWeaponImage);
    void EquipWeaponInHand();
    void StoreWeaponOnBack();
    void DropWeapon();
    void GunTriggerPressed();
    void GunTriggerRelease();
    void ReloadGun();
}

public interface IInteract {

    void Interact();
}