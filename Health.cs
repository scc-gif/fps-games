using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public bool isInstanceDie;
    public float maxHealth ;
    public GameObject EnemyPrefab;
    public float z=15f;
    public float xmin = -7f;
    public float xmax = 7f;
    public float randomX;

    public bool isDead { get; private set; }

    private float _currentHealth;

    private MeshRenderer[] _meshes;
    private Animator _animator;
    private Enemy _enemy;
    private PlayerCharacterController _playerCharacterController;

    public float CurrentHealth {
        get => _currentHealth;
        set => _currentHealth = value;
    }

    private void Awake()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
        _currentHealth = maxHealth;
        _animator=GetComponentInChildren<Animator>();
        _enemy = GetComponent<Enemy>();
        _playerCharacterController=FindObjectOfType<PlayerCharacterController>();
        foreach (MeshRenderer mesh in _meshes)
        {
            mesh.material.color = Color.white;
        }
        ReHealth();
    }

    public void ReHealth() 
    {
        if (_enemy) 
        {
            if (_enemy.enemyType == Enemy.Type.Melee)
                maxHealth = 500;
            else if (_enemy.enemyType == Enemy.Type.Range)
                maxHealth = 1000;
            else if(_enemy.enemyType == Enemy.Type.Melee)
                maxHealth = 5000;
        }    
    }

    public void TakeDamage(float damage) { 
        _currentHealth -= damage;
        _currentHealth= Mathf.Clamp( _currentHealth, 0, maxHealth );

        Debug.Log("CurrentHealth:" + _currentHealth);

        StartCoroutine(OnDamage());
    }

    IEnumerator OnDamage() {
        foreach (MeshRenderer mesh in _meshes) {
            mesh.material.color = Color.red;

        }
        yield return new WaitForSeconds(0.1f);

        if (_currentHealth > 0) {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        else if (!isDead) {
            randomX = Random.Range(8f + xmin, 8f + xmax);
            Vector3 enemyPosition;
            Quaternion enemyRotation = Quaternion.Euler(0, -180, 0);
          
            if (_enemy && _enemy.enemyType == Enemy.Type.Melee)
            {
                _playerCharacterController._MeleeNum++;
                z = 15f;
                enemyPosition = new Vector3(randomX, 0f, z);
                if (_playerCharacterController._MeleeNum < 5) {
                    GameObject newenemy = Instantiate(EnemyPrefab, enemyPosition, enemyRotation);
                }
            }
            else if (_enemy && _enemy.enemyType == Enemy.Type.Range)
            {
                _playerCharacterController._RangeNum++;
                z = 45f;
                enemyPosition = new Vector3(randomX, 0f, z);
                if (_playerCharacterController._RangeNum < 5)
                {
                    GameObject newenemy = Instantiate(EnemyPrefab, enemyPosition, enemyRotation);
                }
            }
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.material.color = Color.gray;
            }

            isDead = true;

            if (isInstanceDie)
            {
                Destroy(gameObject);
            }
            else 
            {
                _animator.SetTrigger("doDie");
                Destroy(gameObject, 3);
            }





            
        }
    }
}
