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
    public LayerMask hittableLayers = -1;  //���в㶼Ҫ������ײ

    public float damage = 20f; 

    //Impact VFX
    public GameObject impactVFX;  //��ײ������Ч��Ԥ���� 
    public float impactVFXLifeTime = 5f;
    public float impactVFXSpawnOffset = 0.1f;  //�ӵ�����ǽ��ʱ ���ӱ�ʵ����֮����ʾ��ƫ����

    public float trajectoryCorrectionDistance = 5f; //ÿһ֡����Ҫ�ƶ��ĳ���

    private ProjectileBase _projectileBase;
    private Vector3 _velocity;
    private Vector3 _lastRootPosition;  //ÿ���������ʼλ��

    private bool _hasTrajectoryCorrected;   //��ʾ׼���Ƿ��Ѿ������У׼
    private Vector3 _correctionVector;  //Onshot�����л�ȡ�Ľ���У׼�ķ�������
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
            //�����������λ�ó���ǹ�ڵ�λ��
            Vector3 cameraToMuzzle = _projectileBase.initialPosition - weaponCameraTransform.position;
            //Debug.DrawRay(weaponCameraTransform.position, cameraToMuzzle, Color.yellow,6);
            
            //ƫ�Ƶ�����
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
