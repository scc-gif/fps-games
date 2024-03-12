using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image bossHealthBarImage;

    private Health _health;

    private void Start()
    {
        Boss boss=FindObjectOfType<Boss>();
        if (boss) 
        {
            _health = boss.GetComponent<Health>();
        }
    }

    private void Update()
    {
        if (_health) 
        {
            bossHealthBarImage.fillAmount = _health.CurrentHealth / _health.maxHealth;
        }
    }
}
