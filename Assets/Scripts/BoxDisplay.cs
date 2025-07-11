using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class BoxDisplay : MonoBehaviour
{
    [Header("制御するオブジェクト")]
    [Tooltip("表示・非表示を制御したい3Dオブジェクトをここに設定します。")]
    public GameObject controlledObject;

    [Header("表示設定")]
    [Tooltip("オブジェクトが表示されてから、消えるまでの時間（秒）")]
    public float timeToDisappear = 5.0f;

    [Header("クールダウン設定")]
    [Tooltip("オブジェクトが消えた後、次に表示されるまでの待ち時間（秒）")]
    public float cooldownDuration = 3.0f; // デフォルトは3秒

    // --- プライベート変数 ---
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;

    public float timer = 0f; // 表示時間をカウントするタイマー

    // ▼▼▼【変更点】クールダウン関連の変数を追加 ▼▼▼
    private bool isCooldown = false; // 現在クールダウン中かどうかのフラグ
    private float cooldownTimer = 0f; // クールダウン時間をカウントするタイマー
    // ▲▲▲【変更点】ここまで ▲▲▲

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
        if (newStatus.Status == Status.TRACKED)
        {
            if (!isTracking)
            {
                OnTrackingFound();
            }
            isTracking = true;
        }
        else
        {
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
        // ▼▼▼【変更点】クールダウン中でない場合のみ表示処理を行う ▼▼▼
        if (isCooldown)
        {
            Debug.Log($"クールダウン中です。表示できません。（残り: {(cooldownDuration - cooldownTimer).ToString("F1")}秒）");
            return; // クールダウン中は何もしない
        }
        // ▲▲▲【変更点】ここまで ▲▲▲

        Debug.Log("マーカーを検出しました。オブジェクトを表示します。");

        if (controlledObject != null)
        {
            controlledObject.SetActive(true);
            timer = 0f; // 表示タイマーをリセット
        }
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
        // オブジェクトが表示されている間、表示タイマーを進める
        if (controlledObject != null && controlledObject.activeSelf)
        {
            timer += Time.deltaTime;

            float remainingTime = timeToDisappear - timer;
            Debug.Log($"オブジェクトが消えるまであと: {remainingTime.ToString("F1")} 秒");

            // 表示時間が経過したら
            if (timer >= timeToDisappear)
            {
                Debug.LogWarning($"設定時間 ({timeToDisappear}秒) が経過しました。オブジェクトを非表示にします。");
                controlledObject.SetActive(false);

                // ▼▼▼【変更点】クールダウンを開始する ▼▼▼
                isCooldown = true;
                cooldownTimer = 0f; // クールダウンタイマーをリセット
                Debug.Log($"クールダウンを開始します。({cooldownDuration}秒)");
                // ▲▲▲【変更点】ここまで ▲▲▲
            }
        }

        // ▼▼▼【変更点】クールダウン中の処理を追加 ▼▼▼
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;
            // クールダウン時間が経過したら
            if (cooldownTimer >= cooldownDuration)
            {
                isCooldown = false; // クールダウンフラグを解除
                Debug.Log("クールダウンが終了しました。再度表示可能です。");
            }
        }
        // ▲▲▲【変更点】ここまで ▲▲▲
    }
}