using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public int attackPower = 0; // ダメージ量は攻撃者から設定される

    private void OnTriggerEnter(Collider other)
    {
        // 接触した相手からDragonControllerを探す
        DragonController target = other.GetComponent<DragonController>();

        // 相手がDragonControllerを持っていれば（＝ドラゴンなら）
        if (target != null)
        {
            // ダメージを与える
            target.TakeDamage(attackPower);

            // 役目を終えたので、この当たり判定オブジェクト自体を消滅させる
            // これにより、1ヒットで必ず消えるので多段ヒットを防げる
            Destroy(gameObject);
        }
    }
}