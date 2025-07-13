using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    public static GameManager instance;

    // ★追加：ゲームが開始したかを管理する、どこからでもアクセスできる旗印
    public static bool isGameStarted = false;

    private bool isRedTowerVisible = false;
    private bool isBlueTowerVisible = false;
    private bool isCountdownStarted = false;

    void Awake()
    {
        // もしinstanceがまだ設定されていなければ、自分自身を代入する
        if (instance == null)
        {
            instance = this;
        }
        // もし既にinstanceが存在していたら、重複しないようにこのオブジェクトを破壊する
        else
        {
            Destroy(gameObject);
        }
    }

    // 赤タワーが認識されたときに呼ばれるメソッド
    public void OnRedTowerFound()
    {
        isRedTowerVisible = true;
        Debug.Log("Red Tower Found!");
        CheckTowersAndStartCountdown();
    }

    // 赤タワーが見失われたときに呼ばれるメソッド
    public void OnRedTowerLost()
    {
        isRedTowerVisible = false;
        Debug.Log("Red Tower Lost!");
        StopAllCoroutines();
        countdownText.gameObject.SetActive(false);
        isCountdownStarted = false;
        isGameStarted = false; // ★追加：ゲーム状態をリセット
    }

    // 青タワーが認識されたときに呼ばれるメソッド
    public void OnBlueTowerFound()
    {
        isBlueTowerVisible = true;
        Debug.Log("Blue Tower Found!");
        CheckTowersAndStartCountdown();
    }

    // 青タワーが見失われたときに呼ばれるメソッド
    public void OnBlueTowerLost()
    {
        isBlueTowerVisible = false;
        Debug.Log("Blue Tower Lost!");
        StopAllCoroutines();
        countdownText.gameObject.SetActive(false);
        isCountdownStarted = false;
        isGameStarted = false; // ★追加：ゲーム状態をリセット
    }

    private void CheckTowersAndStartCountdown()
    {
        if (isRedTowerVisible && isBlueTowerVisible && !isCountdownStarted)
        {
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        isCountdownStarted = true;
        isGameStarted = false; // ★追加：カウントダウン中はゲーム未開始
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "Game Start!";
        isGameStarted = true; // ★追加：ゲーム開始の旗を立てる！

        yield return new WaitForSeconds(1.5f);
        countdownText.gameObject.SetActive(false);

        Debug.Log("ゲーム開始！");
    }
    // ★★★ このメソッドをGameManager.csに追加 ★★★
    public void HandleGameOver(string destroyedTowerTag)
    {
        // ゲームを停止させる
        isGameStarted = false;

        // countdownTextを再利用して勝敗メッセージを表示する
        countdownText.gameObject.SetActive(true);

        if (destroyedTowerTag == "Red Tower")
        {
            countdownText.text = "Player Blue Win!!";
            countdownText.color = Color.blue; // 青文字
        }
        else if (destroyedTowerTag == "Blue Tower")
        {
            countdownText.text = "Player Red Win!!";
            countdownText.color = Color.red; // 赤文字
        }
    }
}