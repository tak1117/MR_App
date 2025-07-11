using UnityEngine;

/// <summary>
/// タワーの表示とステータスを管理するクラス
/// </summary>
public class TowerDisp : MonoBehaviour
{
    [Header("タワーのステータス")]
    [Tooltip("このタワーの初期ヒットポイント（HP）を設定します。")]
    public int maxHp = 100; // HPを設定するための変数。publicなのでインスペクタに表示される
    public int currentHp = 100;

    // ゲームが開始された時に一度だけ呼ばれる
    void Start()
    {
        // 設定したHPをコンソールに出力して確認する
        Debug.Log(gameObject.name + " のHPが " + maxHp + " に設定されました。");
    }

}