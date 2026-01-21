using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] public float playerHP = 100;

    public void TakeDamage(float damage){ playerHP -= damage; }

    public void RestoreHealt(float HP) { playerHP += HP; }

    public void Update() 
    {
        if (playerHP <= 0)
        {
            SceneDeath death = gameObject.GetComponent<SceneDeath>();
            death.PlayerDeath();
        }
    }
}
