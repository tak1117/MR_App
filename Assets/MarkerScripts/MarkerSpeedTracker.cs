using UnityEngine;
using Vuforia;

/// <summary>
/// VuforiaのARマーカーの移動速度を計算する（改善版）
/// 一定時間ごとにサンプリングして、微小な動きでも速度を検出しやすくする
/// </summary>
public class MarkerSpeedTracker : MonoBehaviour
{
    [Header("現在の速度 (m/s)")]
    [Tooltip("計算されたマーカーの現在の速度がここに表示されます。")]
    public float speed = 0f;

    [Header("設定")]
    [Tooltip("速度を計算する間隔（秒）。小さいほど頻繁に更新されるが、ブレに弱くなる。")]
    public float sampleInterval = 0.1f; // 0.1秒ごとに速度を計算

    // --- プライベート変数 ---
    private Vector3 previousPosition;
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;
    private float sampleTimer = 0f; // サンプリング用のタイマー

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
    }

    void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
    
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus newStatus)
    {
        if (newStatus.Status == Status.TRACKED)
        {
            if (!isTracking)
            {
                // 追跡開始時に、現在位置とタイマーをリセット
                previousPosition = transform.position;
                sampleTimer = 0f;
            }
            isTracking = true;
        }
        else
        {
            isTracking = false;
            speed = 0f;
        }
    }

    void Update()
    {
        // マーカーが追跡中でなければ何もしない
        if (!isTracking) return;
        
        // サンプリング用のタイマーを進める
        sampleTimer += Time.deltaTime;

        // 設定した計算間隔（sampleInterval）を過ぎたら、速度を計算する
        if (sampleTimer >= sampleInterval)
        {
            // 移動した距離を計算
            float distance = Vector3.Distance(transform.position, previousPosition);

            // 速度を計算 (速度 = 距離 / 経過時間)
            speed = distance / sampleTimer;

            // コンソールに速度を出力
            Debug.Log($"マーカーの速度: {speed.ToString("F2")} m/s");

            // 現在の位置とタイマーを次の計算のためにリセット
            previousPosition = transform.position;
            sampleTimer = 0f;
        }
    }
}