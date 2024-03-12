using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController Instance;
    public GameObject door1,door2;

    public Camera playerCamera;
    public float gravityDownForce = 20f;
    public float maxSpeedOnGround = 8f;
    public float moveSharpnessOnGround = 15f;
    public float rotationSpeed = 200f;

    public float maxHealth = 200f;
    public int _MeleeNum = 0, _RangeNum = 0,level=1;

    public float cameraHeightRatio = 0.9f;

    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private float _targetCharacterHeight = 1.8f;
    private float _cameraVerticalAngle = 0f;
    private float _currentHealth;
    private bool _isBossAttack;

    public float CurrentHealth => _currentHealth;
    
    public Vector3 CharacterVelocity { get; set; }
    
    private void Awake()
    {
        Instance = this;
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();

        _characterController.enableOverlapRecovery = true;
        level = 1;

        UpdateCharacterHeight();
    }

    private void Update()
    {
        HandleCharacterMovement();
        Updatelevels();

    }

    private void Updatelevels() {
        if (_MeleeNum >= 5)
        {
            level = 2;
            if (door1) {
                Destroy(door1, 1);
            }
        }
        if (_RangeNum >= 5)
        {
            level = 3;
            if (door2)
            {
                Destroy(door2, 1);
            }
        }

    }

    private void UpdateCharacterHeight()
    {
        _characterController.height = _targetCharacterHeight;
        _characterController.center = Vector3.up * _characterController.height * 0.5f;

        playerCamera.transform.localPosition = Vector3.up * _characterController.height * 0.9f;
    }

    // ReSharper restore Unity.ExpensiveCode
    private void HandleCharacterMovement()
    {
        // Camera rotate horizontal
        transform.Rotate(new Vector3(0, _inputHandler.GetMouseLookHorizontal() * rotationSpeed, 0), 
            Space.Self);
        
        // Camera rotate vertical
        _cameraVerticalAngle += _inputHandler.GetMouseLookVertical() * rotationSpeed;

        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(-_cameraVerticalAngle, 0, 0);
        
        // Move 
        Vector3 worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());

        if (_characterController.isGrounded)
        {
            Vector3 targetVelocity = worldSpaceMoveInput * maxSpeedOnGround;

            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                moveSharpnessOnGround * Time.deltaTime);
        }
        else
        {
            CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        if (_isBossAttack) 
        {
            CharacterVelocity += transform.forward * -1;
        }

        _characterController.Move(CharacterVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHitPlayer(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        OnHitPlayer(other.collider);
    }

    private void OnHitPlayer(Collider other) {
        if (other.CompareTag("EnemyBullet")) 
        { 
            Bullet enemyBullet = other.GetComponent<Bullet>();
            _currentHealth -= enemyBullet.damage;

            StartCoroutine(OnDamage());
            if (other.GetComponent<Rigidbody>()) 
            {
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("MeleeArea")) 
        {
            MeleeAttacker attacker = other.GetComponent<MeleeAttacker>();

            _currentHealth -= attacker.damage;
            print("Player got hit by enemy melee");

            _isBossAttack = other.name == "Boss Melee Area";
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() 
    {
        print("Current Player Health:" + _currentHealth);

        if (_currentHealth <= 0) 
        {
            OnDie();
        }
        yield return new WaitForSeconds(0.2f);
        _isBossAttack = false;
    }

    private void OnDie() 
    {
        SceneManager.LoadScene("Level1");
    }

}
