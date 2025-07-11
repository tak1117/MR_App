using UnityEngine;
using Vuforia;

/// <summary>
/// ARマーカーを認識後、一定時間経過したらオブジェクトを非表示にするコントローラー
/// </summary>
public class BoxDisp : MonoBehaviour
{
    [Header("制御するオブジェクト")]
    [Tooltip("表示・非表示を制御したい3Dオブジェクトをここに設定します。")]
    public GameObject controlledObject;

    [Header("オブジェクトが消えるまでの時間 (秒)")]
    [Tooltip("オブジェクトが表示されてから、ここに設定した秒数が経過すると非表示になります。")]
    public float timeToDisappear = 5.0f; // デフォルトは5秒

    // プライベート変数
    private bool isTracking = false;
    private ObserverBehaviour observerBehaviour;
    public float timer = 0f; // 経過時間をカウントするタイマー

    private BoxDisp boxDispInstance;

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
            // 追跡が始まった瞬間
            if (!isTracking)
            {
                OnTrackingFound();
            }
            isTracking = true;
        }
        // マーカーが見えなくなった場合
        else
        {
            // 追跡がロストした瞬間
            if (isTracking)
            {
                OnTrackingLost();
            }
            isTracking = false;
        }
    }

    /// <summary>
    /// マーカーの追跡が開始された時の処理
    /// </summary>
    private void OnTrackingFound()
    {
        Debug.Log("マーカーを検出しました。オブジェクトを表示します。");

        if (controlledObject != null)
        {
            // オブジェクトを表示する
            controlledObject.SetActive(true);
            // タイマーをリセットしてカウント開始
            timer = 0f;
        }
    }

    /// <summary>
    /// マーカーの追跡が失われた時の処理
    /// </summary>
    private void OnTrackingLost()
    {
        Debug.Log("マーカーがロストしました。");
        if (controlledObject != null)
        {
            // オブジェクトを非表示にする
            controlledObject.SetActive(false);
        }
    }

    void Update()
    {
        // オブジェクトが表示されている間だけタイマーを進める
        if (controlledObject != null && controlledObject.activeSelf)
        {
            // 経過時間を加算
            timer += Time.deltaTime;

            // デバッグ用に残り時間をコンソールに出力
            float remainingTime = timeToDisappear - timer;
            Debug.Log($"オブジェクトが消えるまであと: {remainingTime.ToString("F1")} 秒");

            // タイマーが設定時間を超えたら
            if (timer >= timeToDisappear)
            {
                Debug.LogWarning($"設定時間 ({timeToDisappear}秒) が経過しました。オブジェクトを非表示にします。");
                // オブジェクトを非表示にする
                controlledObject.SetActive(false);
            }
        }
    }
}