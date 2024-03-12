using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : MonoBehaviour
{
    public float maxLifeTime = 5f;
    public float speed = 300f;

    public Transform root;
    public Transform tip;
    public float radius = 0.01f;
    public LayerMask hittableLayers = -1;  //所有层都要进行碰撞

    public float damage = 20f; 

    //Impact VFX
    public GameObject impactVFX;  //碰撞的粒子效果预制体 
    public float impactVFXLifeTime = 5f;
    public float impactVFXSpawnOffset = 0.1f;  //子弹击打墙壁时 粒子被实例化之后显示的偏移量

    public float trajectoryCorrectionDistance = 5f; //每一帧具体要移动的长度

    private ProjectileBase _projectileBase;
    private Vector3 _velocity;
    private Vector3 _lastRootPosition;  //每次射击的起始位置

    private bool _hasTrajectoryCorrected;   //表示准星是否已经完成了校准
    private Vector3 _correctionVector;  //Onshot函数中获取的将被校准的方向向量
    private Vector3 _consumedCorrectionVector;

    private void OnEnable()
    {
        _projectileBase = GetComponent<ProjectileBase>();
        _projectileBase.onShoot += OnShoot;
        Destroy(gameObject, maxLifeTime);
    }

    private void OnShoot() { 
        _lastRootPosition = root.position;
        _velocity+=transform.forward*speed;

        PlayerWeaponManager playerWeaponManager = _projectileBase.owner.GetComponent<PlayerWeaponManager>();

        if (playerWeaponManager) {
            _hasTrajectoryCorrected = false;

            Transform weaponCameraTransform = playerWeaponManager.weaponCamera.transform;
            //武器摄像机的位置朝向枪口的位置
            Vector3 cameraToMuzzle = _projectileBase.initialPosition - weaponCameraTransform.position;
            //Debug.DrawRay(weaponCameraTransform.position, cameraToMuzzle, Color.yellow,6);
            
            //偏移的向量
            _correctionVector = Vector3.ProjectOnPlane(-cameraToMuzzle, weaponCameraTransform.forward);
            
        
        
        }


    }

    private void Update()
    {
        //Move
        transform.position+=_velocity*Time.deltaTime;

        //Orient
        transform.forward = _velocity.normalized;

        //Drift the projectile to camera center
        if (!_hasTrajectoryCorrected && _consumedCorrectionVector.sqrMagnitude < _correctionVector.sqrMagnitude)
        { 
            //how much correction vector left to be consumed for accuracy adjustment
            Vector3 correctionLeft = _correctionVector - _consumedCorrectionVector;

            float distanceThisFrame = (root.position - _lastRootPosition).magnitude;

            Vector3 correctionThisFrame = (distanceThisFrame / trajectoryCorrectionDistance) * _correctionVector;

            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            _consumedCorrectionVector += correctionThisFrame;

            if (Mathf.Abs(_consumedCorrectionVector.sqrMagnitude - _correctionVector.sqrMagnitude) < Mathf.Epsilon) 
            {
                _hasTrajectoryCorrected = true;
            }

            //Start drifting the projectile
            transform.position += correctionThisFrame;
        }
        //Hit detection
        RaycastHit closestHit = new RaycastHit();
        closestHit.distance = Mathf.Infinity;
        bool foundHit = false;



        //SphereCastAll
        Vector3 displacementSinceLastFrame = tip.position - _lastRootPosition;

        RaycastHit[] hits = Physics.SphereCastAll(_lastRootPosition,
            radius,
            displacementSinceLastFrame.normalized,
            displacementSinceLastFrame.magnitude,
            hittableLayers,
            QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits) {
            if (IsHitValid(hit) && hit.distance < closestHit.distance) {
                closestHit = hit;
                foundHit = true;
            }
        }

        if (foundHit) {
            if (closestHit.distance <= 0)
            {
                closestHit.point = root.position;
                closestHit.normal = -transform.forward;
            }

            OnHit(closestHit.point,closestHit.normal,closestHit.collider);
        }


    }

    private bool IsHitValid(RaycastHit hit) {
        if (hit.collider.isTrigger) {
            return false;
        }
        return true;
    
    }

    private void OnHit(Vector3 point,Vector3 normal,Collider collider) {
        Damageable damageable=collider.GetComponent<Damageable>();

        if (damageable) {
            damageable.InflictDamage(damage);
        
        }



        if (impactVFX != null)
        {
            GameObject impactVFXInstance=Instantiate(impactVFX,
                point+normal* impactVFXSpawnOffset,
                Quaternion.LookRotation(normal));

            if (impactVFXLifeTime > 0) {
                Destroy(impactVFXInstance, impactVFXLifeTime); 
            }
        }
        print("Hit");

        Destroy(gameObject);
        
    }

}
