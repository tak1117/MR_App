using UnityEngine;

public class IVCamViewer : MonoBehaviour
{
    WebCamTexture webcamTexture;

    void Start()
    {
        // 接続されているWebカメラ一覧を取得
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                Debug.Log("デバイス名: " + devices[i].name);
            }

            // 例: iVCamを指定して起動（自動で最初のカメラでもOK）
            string ivcamName = devices[0].name; // 必要に応じて選ぶ
            webcamTexture = new WebCamTexture(ivcamName);
            GetComponent<Renderer>().material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        else
        {
            Debug.LogWarning("Webカメラが見つかりませんでした。");
        }
    }
}
