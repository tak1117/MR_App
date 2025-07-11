using UnityEngine;
using TMPro; // TextMeshProを使用する場合
using System.Collections;

public class GameManager : MonoBehaviour
{
    // インスペクターからUIテキストをアタッチする
    public TextMeshProUGUI countdownText;

    // 各タワーがスポーンしたかを管理するフラグ
    private bool isRedTowerVisible = false;
    private bool isBlueTowerVisible = false;

    // カウントダウンが既に開始されたかを管理するフラグ
    private bool isCountdownStarted = false;

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
        // カウントダウン中にマーカーが見失われたら中断する
        StopAllCoroutines();
        countdownText.gameObject.SetActive(false);
        isCountdownStarted = false; // リセット
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
        // カウントダウン中にマーカーが見失われたら中断する
        StopAllCoroutines();
        countdownText.gameObject.SetActive(false);
        isCountdownStarted = false; // リセット
    }

    // 両方のタワーが表示されているかチェックする
    private void CheckTowersAndStartCountdown()
    {
        // 両方のタワーが表示されていて、まだカウントダウンが始まっていなければ開始
        if (isRedTowerVisible && isBlueTowerVisible && !isCountdownStarted)
        {
            StartCoroutine(StartCountdown());
        }
    }

    // カウントダウンを実行するコルーチン
    private IEnumerator StartCountdown()
    {
        isCountdownStarted = true;
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "Game Start!";
        yield return new WaitForSeconds(1.5f); // 1.5秒表示

        countdownText.gameObject.SetActive(false);

        // --- ここにゲーム開始後の処理を記述 ---
        Debug.Log("ゲーム開始！");
    }
}