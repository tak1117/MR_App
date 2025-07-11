using UnityEngine;
using UnityEngine.UI;
public class HPDisp : MonoBehaviour
{
    // HPバーのスライダー
    [SerializeField]
    private Slider hpSlider;
    // メインカメラ
    private Camera mainCamera;
    [SerializeField] private BoxDisp boxDispInstance;
    void Start()
    {
        boxDispInstance = GetComponent<BoxDisp>();
        // メインカメラをキャッシュしておく
        mainCamera = Camera.main;
    }
    // Updateの後に呼ばれるLateUpdateを使い、カメラの動きに追従させる
    void LateUpdate()
    {
        // HPバーが常にカメラの方向を向くようにする
        if (mainCamera != null)
        {
            hpSlider.transform.rotation = mainCamera.transform.rotation;
        }
    }
    // HPの値を更新する公のメソッド
    public void UpdateHP(float currentHp, float maxHp)
    {
        // Sliderの値を0から1の範囲で設定する
        hpSlider.value = 1-currentHp / maxHp;
    }

    void Update()
    {
        float currentHp = boxDispInstance.timer;
        float maxHp = boxDispInstance.timeToDisappear;
        UpdateHP(currentHp, maxHp);
     }
}