using UnityEngine;
using UnityEngine.UI;

public class HpDisplay : MonoBehaviour
{
    // HPバーのスライダー
    [SerializeField]
    protected Slider hpSlider;
    // メインカメラ
    protected Camera mainCamera;
    [SerializeField] private BoxDisplay boxDispInstance;

    void Start()
    {
        boxDispInstance = GetComponent<BoxDisplay>();
        // メインカメラをキャッシュしておく
        mainCamera = Camera.main;
    }

    // Updateの後に呼ばれるLateUpdateを使い、カメラの向きに追従させる
    void LateUpdate()
    {
        // 追記：hpSliderがnullでないことを確認する
        // HPバーとカメラの向きを同期させる処理
        if (hpSlider != null && mainCamera != null)
        {
            hpSlider.transform.rotation = mainCamera.transform.rotation;
        }
    }

    // HPの値を更新するためのメソッド
    public void UpdateHP(float currentHp, float maxHp)
    {
        // 追記：hpSliderがnullでないことを確認する
        // HPバーが存在する場合に処理を行う
        if (hpSlider != null)
        {
            // Sliderの値を0から1の範囲で設定する
            hpSlider.value = currentHp / maxHp;
        }
    }

    void Update()
    {
        // boxDispInstanceのタイマーをHPとして扱う
        float currentHp = boxDispInstance.timer;
        float maxHp = boxDispInstance.timeToDisappear;
        UpdateHP(currentHp, maxHp);
    }
}