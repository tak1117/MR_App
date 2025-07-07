using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    // HPバーのスライダー
    [SerializeField]
    private Slider hpSlider;

    // メインカメラ
    private Camera mainCamera;

    void Start()
    {
        // メインカメラをキャッシュしておく
        mainCamera = Camera.main;
    }

    // Updateの後に呼ばれるLateUpdateを使い、カメラの動きに追従させる
    void LateUpdate()
    {
        // HPバーが常にカメラの方向を向くようにする
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
    }

    // HPの値を更新する公のメソッド
    public void UpdateHP(int currentHp, int maxHp)
    {
        // Sliderの値を0から1の範囲で設定する
        hpSlider.value = (float)currentHp / maxHp;
    }
}