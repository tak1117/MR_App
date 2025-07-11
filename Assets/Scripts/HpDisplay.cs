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

    // Updateの後に呼ばれるLateUpdateを使い、カメラの動きに追従させる
    void LateUpdate()
    {
        // ★ 修正点: hpSliderがnullでないことも確認する
        // HPバーとカメラの両方が存在する時だけ処理する
        if (hpSlider != null && mainCamera != null)
        {
            hpSlider.transform.rotation = mainCamera.transform.rotation;
        }
    }

    // HPの値を更新する公のメソッド
    public void UpdateHP(float currentHp, float maxHp)
    {
        // ★ 修正点: hpSliderがnullでないことを確認する
        // HPバーが存在する時だけ処理する
        if (hpSlider != null)
        {
            // Sliderの値を0から1の範囲で設定する
            hpSlider.value = currentHp / maxHp;
        }
    }

    void Update()
    {
        // この部分は、hpSliderのnullチェックがUpdateHP内で行われるため修正不要
        float currentHp = boxDispInstance.timer;
        float maxHp = boxDispInstance.timeToDisappear;
        UpdateHP(currentHp, maxHp);
    }
}