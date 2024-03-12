using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBarImage;

    private PlayerCharacterController _playerCharacterController;

    private void Start()
    {
        _playerCharacterController =FindObjectOfType<PlayerCharacterController>();
    }

    private void Update()
    {
        if (_playerCharacterController) 
        {
            healthBarImage.fillAmount = _playerCharacterController.CurrentHealth / _playerCharacterController.maxHealth;
        }
    }
}
