using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponRoot;

    public Transform weaponMuzzle;
    public ProjectileBase projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public float delayBetweenShots = 0.1f; //射击的频率

    private float _lastShotTime = Mathf.NegativeInfinity; //上次射击时间 先定义负无限大的值
    public Vector3 muzzleWorldVelocity { get; private set; }
    public bool isWeaponActive { get; private set; }
    public GameObject owner { get; set; }
    public GameObject sourcePrefab { get; set; }
    public void showWeapon(bool show) {
        weaponRoot.SetActive(show);
        isWeaponActive = true; 
    }

    public bool HandleShootInputs(bool inputHeld) {
        if (inputHeld) {
            return TryShoot();
        }

        return false;
    
    }
    private bool TryShoot() {
        if (_lastShotTime + delayBetweenShots < Time.time) {
            HandleShoot();
            print("Shot!");
            return true;
        }
        return false;
    }

    private void HandleShoot() {
        if (projectilePrefab != null) {
            Vector3 shotDirection=weaponMuzzle.forward;
            ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                weaponMuzzle.rotation, weaponMuzzle.transform);
            newProjectile.Shoot(this);
        }
        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                weaponMuzzle.rotation,weaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2);
        }
        _lastShotTime = Time.time;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
