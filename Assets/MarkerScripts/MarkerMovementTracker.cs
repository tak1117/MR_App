using UnityEngine;
using Vuforia;

/// <summary>
/// ARマーカーの総移動距離を計算して表示するクラス
/// </summary>
public class MarkerMovementTracker : MonoBehaviour
{
    [Header("総移動距離 (m)")]
    [Tooltip("マーカーが追跡開始から移動した合計の距離が表示されます。")]
    public float totalDistanceMoved = 0f;

    // 1フレーム前の位置を保存するための変数
    private Vector3 previousPosition;

    // Vuforiaのトラッキング状態を管理するコンポーネント
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;

    [SerializeField] private MarkerSpeedTracker speedTracker;
    [SerializeField] private TowerDisp Tower;
    public float minSpeed = 60;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
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
            // 追跡が始まった瞬間に、各種値をリセット
            if (!isTracking)
            {
                totalDistanceMoved = 0f;
                previousPosition = transform.position;
            }
            isTracking = true;
        }
        else
        {
            isTracking = false;
        }
    }

    void Update()
    {
        // マーカーが追跡中でなければ何もしない
        if (!isTracking) return;
        
        // 1フレームの間に移動した距離を計算
        float frameDistance = Vector3.Distance(transform.position, previousPosition);

        // 総移動距離に加算していく
        if (speedTracker.speed > minSpeed)
        {
            totalDistanceMoved += frameDistance;
            if (Tower.currentHp >= 0)
            {
                Tower.currentHp -= frameDistance/20;
            }
            
         }

        
        // コンソールに総移動距離を出力
        Debug.Log($"総移動距離: {totalDistanceMoved.ToString("F2")} m");

        // 現在の位置を「1フレーム前の位置」として更新する
        previousPosition = transform.position;
    }
}