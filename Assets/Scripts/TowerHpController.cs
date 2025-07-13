using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHpController : MonoBehaviour
{
    [SerializeField] TowerDisplay towerDisplay;
    private bool isDestroyed = false;
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;
        towerDisplay.currentHp -= damage;
        if (towerDisplay.currentHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // GameManagerに破壊されたことを通知する
        if (GameManager.instance != null)
        {
            GameManager.instance.HandleGameOver(gameObject.tag);
        }

        Destroy(gameObject);
    }
}
