using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponManager : MonoBehaviour
{
    public List<WeaponController> startingWeapons =new List<WeaponController> ();

    public Camera weaponCamera;
    public Transform weaponParentSocket;
    public UnityAction<WeaponController> onSwitchToWeapon;

    private WeaponController[] _weaponSlots = new WeaponController[9];



    // Start is called before the first frame update
    private void Start()
    {
        onSwitchToWeapon += OnWeaponSwitched;

        foreach (WeaponController weapon in startingWeapons) {
            AddWeapon(weapon);
        }

        SwitchWeapon();

    }

    private void Update()
    {
        WeaponController activeWeapon = _weaponSlots[0];
        if (activeWeapon) {
            activeWeapon.HandleShootInputs(PlayerInputHandler.Instance.GetFireInputHeld());
        }
    }

    public bool AddWeapon(WeaponController weaponPrefab) {
        for (int i = 0; i < _weaponSlots.Length; i++) { 
            WeaponController weaponInstance=Instantiate(weaponPrefab,weaponParentSocket);
            weaponInstance.transform.localPosition= Vector3.zero;
            weaponInstance.transform.localRotation= Quaternion.identity;

            weaponInstance.owner = gameObject;
            weaponInstance.sourcePrefab = weaponPrefab.gameObject;
            weaponInstance.showWeapon(false);

            _weaponSlots[i]=weaponInstance;

            return true;
        }
        return false;
    }

    public void SwitchWeapon() {
        SwitchWeaponToIndex(0);
    }

    public void SwitchWeaponToIndex(int newWeaponIndex) {
        if (newWeaponIndex >= 0) {
            WeaponController newWeapon = GetWeaponAtSlotIndex(newWeaponIndex);

            if (onSwitchToWeapon != null) {
                onSwitchToWeapon.Invoke(newWeapon);
            }
        }
    }

    public WeaponController GetWeaponAtSlotIndex(int index) {
        if (index >= 0 && index < _weaponSlots.Length) {
            return _weaponSlots[index];
        }
        return null;
    }

    private void OnWeaponSwitched(WeaponController newWeapon) {
        if (newWeapon != null) {
            newWeapon.showWeapon(true);
        }
    }

    // Update is called once per frame

}
