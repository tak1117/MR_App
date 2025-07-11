using UnityEngine;
using Vuforia;

/// <summary>
/// ARマーカーを認識後、一定距離移動したらオブジェクトを非表示にするコントローラー
/// （タイミング問題対策＋詳細デバッグ機能付き）
/// </summary>
/// 


public class BoxDisp : MonoBehaviour
{
    [Header("制御するオブジェクト")]
    [Tooltip("表示・非表示を制御したい3Dオブジェクトをここに設定します。")]
    public GameObject controlledObject;

    [Header("距離のしきい値 (メートル単位)")]
    [Tooltip("オブジェクトが非表示になる距離（例: 20cmなら0.2）")]
    public float distanceThreshold = 0.2f;

    public float minDis;

    private float sumDistance;

    private Vector3 initialPosition;
    private bool isTracking = false;
    private ObserverBehaviour observerBehaviour;

    // ▼▼▼【変更点】▼▼▼
    // マーカーを検出した直後かどうかを判定するためのフラグ
    private bool justFound = false;
    // ▲▲▲【変更点】▲▲▲


    private float stableTimer = 0f;
    private float requiredStableTime = 0.5f; // 0.5秒安定してから座標取得

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

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus newStatus)
    {
        if (newStatus.Status == Status.TRACKED || newStatus.Status == Status.EXTENDED_TRACKED)
        {
            if (!isTracking) // 追跡が始まった瞬間
            {
                OnTrackingFound();
            }
            isTracking = true;
        }
        else
        {
            if (isTracking) // 追跡がロストした瞬間
            {
                OnTrackingLost();
            }
            isTracking = false;
        }
    }

    private void OnTrackingFound()
    {
        Debug.Log("マーカーの検出を検知しました。");
        if (controlledObject != null)
        {
            controlledObject.SetActive(true);
        }
        // ▼▼▼【変更点】▼▼▼
        // すぐに座標を記録せず、フラグを立てるだけにする
        justFound = true;
        // ▲▲▲【変更点】▲▲▲
    }

    private void OnTrackingLost()
    {
        Debug.Log("マーカーがロストしました。");
        if (controlledObject != null)
        {
            controlledObject.SetActive(false);
        }
    }

    void Update()
    {
        // マーカーを追跡中でなければ何もしない
        if (!isTracking) return;

        // ▼▼▼【変更点】マーカー検出直後の最初のUpdateフレームで初期位置を確定させる ▼▼▼
        if (justFound)
        {
            stableTimer += Time.deltaTime;

            if (stableTimer >= requiredStableTime)
            {
                initialPosition = transform.position;
                justFound = false;
                stableTimer = 0f;
                Debug.Log($"<color=green>【初期座標を記録】 {initialPosition.ToString("F4")}</color>");
            }
            return;
        }
        // ▲▲▲【変更点】ここまで ▲▲▲

        if (controlledObject != null && controlledObject.activeSelf)
        {
            Vector3 currentPosition = transform.position;
            float distance = Vector3.Distance(currentPosition, initialPosition);
            if (distance > minDis)
            {
                sumDistance += distance;
            }

            // 【デバッグ出力】初期座標、現在座標、距離をすべて表示
            Debug.Log($"<b>現在位置:</b> {currentPosition.ToString("F4")} | <b>初期位置:</b> {initialPosition.ToString("F4")} | <b>距離:</b> {distance.ToString("F2")} m");

            if (distance > distanceThreshold)
            {
                Debug.LogWarning($"距離 ({distance}m) がしきい値 ({distanceThreshold}m) を超えました。オブジェクトを非表示にします。");
                controlledObject.SetActive(false);
            }
        }
    }
}