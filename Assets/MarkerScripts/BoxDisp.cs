using UnityEngine;
using Vuforia;

/// <summary>
/// ARマーカーを初回認識後、一定時間経過したらオブジェクトを非表示にするコントローラー。
/// 一度出現したオブジェクトは、時間経過するまでマーカーの有無に関わらず表示され続ける。
/// </summary>
public class BoxDisp : MonoBehaviour
{
    [Header("制御するオブジェクト")]
    [Tooltip("表示・非表示を制御したい3Dオブジェクトをここに設定します。")]
    public GameObject controlledObject;

    [Header("オブジェクトが消えるまでの時間 (秒)")]
    [Tooltip("オブジェクトが最初に出現してから、ここに設定した秒数が経過すると非表示になります。")]
    public float timeToDisappear = 5.0f;

    // --- プライベート変数 ---
    private ObserverBehaviour observerBehaviour;
    private float timer = 0f; // 経過時間をカウントするタイマー

    /// <summary>
    /// 最初のスポーンが行われたかどうかを判定するフラグ
    /// </summary>
    private bool hasSpawnedOnce = false;

    /// <summary>
    /// タイマーが作動中かどうかを判定するフラグ
    /// </summary>
    private bool isTimerRunning = false;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogError("ObserverBehaviourが見つかりません。ImageTargetにアタッチしてください。");
        }

        // 起動時はオブジェクトを非表示にしておく
        if (controlledObject != null)
        {
            controlledObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    /// <summary>
    /// Vuforiaマーカーのトラッキング状態が変化した時に呼ばれる
    /// </summary>
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus newStatus)
    {
        // マーカーが直接見えている安定した状態の時だけ処理する
        if (newStatus.Status == Status.TRACKED)
        {
            OnTrackingFound();
        }
    }

    /// <summary>
    /// マーカーの追跡が開始された時の処理
    /// </summary>
    private void OnTrackingFound()
    {
        if (controlledObject == null) return;

        // オブジェクトの位置と回転をマーカーに合わせる
        controlledObject.transform.position = this.transform.position;
        controlledObject.transform.rotation = this.transform.rotation;

        // オブジェクトを表示状態にする
        controlledObject.SetActive(true);

        // まだ一度もスポーンしていなければ、タイマーを開始する
        if (!hasSpawnedOnce)
        {
            Debug.Log("初回マーカー検出。タイマースタート！");
            hasSpawnedOnce = true;
            isTimerRunning = true;
            timer = 0f; // タイマーをリセットしてスタート
        }
        else
        {
            Debug.Log("マーカーを再検出。オブジェクトの位置を更新しました。");
        }
    }

    void Update()
    {
        // タイマーが作動中の場合のみ時間を加算する
        if (isTimerRunning)
        {
            timer += Time.deltaTime;

            // タイマーが設定時間を超えたら
            if (timer >= timeToDisappear)
            {
                Debug.LogWarning($"設定時間 ({timeToDisappear}秒) が経過しました。オブジェクトを非表示にします。");

                if (controlledObject != null)
                {
                    // オブジェクトを非表示にする
                    controlledObject.SetActive(false);
                }

                // タイマーを停止する
                isTimerRunning = false;
            }
        }
    }
}