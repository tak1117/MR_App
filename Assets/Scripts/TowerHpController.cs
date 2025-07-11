using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHpController : MonoBehaviour
{
    [SerializeField] TowerDisplay towerDisplay;
    private bool isDestroyed = false;
    public void TakeDamage(float damage)
    {
        if(isDestroyed) return;
        towerDisplay.currentHp -= damage;
        if(towerDisplay.currentHp <= 0 )
        {
            Die();
        }
    }
    private void Die()
    {
        if(isDestroyed) return;
        isDestroyed = true;
        Destroy(gameObject);
    }
}
