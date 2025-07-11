using UnityEngine;
using UnityEngine.UI;
public class towerHPDisp : HPDisp
{

    [SerializeField] private TowerDisp TowerDispInstance;

    public void UpdateHP(float currentHp, float maxHp)
    {
        // Sliderの値を0から1の範囲で設定する
        hpSlider.value = currentHp / maxHp;
    }
    void Update()
    {
        float currentHp = TowerDispInstance.currentHp;
        float maxHp = TowerDispInstance.maxHp;
        UpdateHP(currentHp, maxHp);
     }
}