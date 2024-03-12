using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : Enemy
{
    public bool isLook;

    public GameObject missileBoss;
    public Transform missilePortA;
    public Transform missilePortB;

    public Image _bossHealthBar1;
    public Image _bossHealthBar2;

    private Vector3 _lookVec;
    private BoxCollider _boxCollider;
    private Vector3 _jumpHitTarget;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _health=GetComponent<Health>();
        _boxCollider = GetComponent<BoxCollider>();

        if(_navMeshAgent)
            _navMeshAgent.isStopped= true;

        //StartCoroutine(Think());
    }

    private void Update()
    {
        if (_health.CurrentHealth != _health.maxHealth  && !isalive)
        {
            StartCoroutine(OnbossAlive());
            StartCoroutine(Think());
        }
        if (_health.isDead) 
        {
            StopAllCoroutines();
            GameManager.instance.RestartGame();
        }


        if (isLook)
        {
            float horizon = Input.GetAxisRaw("Horizontal");
            float vectical = Input.GetAxisRaw("Vertical");
            _lookVec = new Vector3(horizon, 0, vectical) * 5f;
            transform.LookAt(target.position + _lookVec);
        }
        else 
        {
            if(_navMeshAgent)
                _navMeshAgent.SetDestination(_jumpHitTarget);
        }

    }

    private IEnumerator MissileShot() 
    {
       
            _animator.SetTrigger("doShot");

            yield return new WaitForSeconds(0.2f);
            GameObject InstantMissileA = Instantiate(missileBoss, missilePortA.position, missilePortA.rotation);
            MissileBoss missileBossA = InstantMissileA.GetComponent<MissileBoss>();
            missileBossA.target = target;

            yield return new WaitForSeconds(0.3f);
            GameObject InstantMissileB = Instantiate(missileBoss, missilePortB.position, missilePortB.rotation);
            MissileBoss missileBossB = InstantMissileB.GetComponent<MissileBoss>();
            missileBossB.target = target;

            yield return new WaitForSeconds(2.5f);

            StartCoroutine(Think());
        
    }

    private IEnumerator JumpHit() 
    {
       
            _jumpHitTarget = target.position + _lookVec;

            isLook = false;
            _navMeshAgent.isStopped = false;
            _boxCollider.enabled = false;

            _animator.SetTrigger("doJumpHit");
            yield return new WaitForSeconds(1.5f);

            meleeArea.enabled = true;

            yield return new WaitForSeconds(0.5f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(1f);

            isLook = true;
            _navMeshAgent.isStopped = true;
            _boxCollider.enabled = true;

            yield return new WaitForSeconds(2f);
            StartCoroutine(Think());
        
    }

    public IEnumerator OnbossAlive()
    {
        isalive = true;
        _navMeshAgent.enabled = true;
        _bossHealthBar1.enabled = true;
        _bossHealthBar2.enabled = true;

        yield return null;

    }

    private IEnumerator Think() 
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = UnityEngine.Random.Range(0, 5);

        switch (ranAction) 
        {
            case 0:case 1: case 2: case 3:
                StartCoroutine(MissileShot());
                break;
            case 4:
                StartCoroutine(JumpHit());
                break;
        }
    
    }
}
