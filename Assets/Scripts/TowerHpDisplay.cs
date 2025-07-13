using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerHpDisplay : HpDisplay
{

    [SerializeField] private TowerDisplay TowerDispInstance;

    void Update()
    {
        float currentHp = TowerDispInstance.currentHp;
        float maxHp = TowerDispInstance.maxHp;
        UpdateHP(currentHp, maxHp);
    }
}