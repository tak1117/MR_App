using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public float attackPower = 0f;
    public string attackerTag; // ★追加：攻撃者のタグを保存する変数

    private void OnTriggerEnter(Collider other)
    {
        // 衝突した相手が DragonBaseController を持っているか試す
        if (other.TryGetComponent<DragonBaseController>(out var dragon))
        {
            // ★追加：当たったドラゴンが味方なら、何もしない
            if (other.CompareTag(attackerTag))
            {
                return;
            }

            // 敵だったので、そのドラゴンの TakeDamage を呼ぶ
            dragon.TakeDamage(attackPower);
            Debug.Log(other.name + " にヒット！");
            Destroy(gameObject); // ヒットボックスを消滅させる
        }
        // 衝突した相手が TowerHpController を持っているか試す
        else if (other.TryGetComponent<TowerHpController>(out var tower))
        {
            // ★追加：当たったタワーが味方かどうかを判定
            bool isFriendlyTower = (attackerTag.Contains("Red") && other.CompareTag("Red Tower")) ||
                                 (attackerTag.Contains("Blue") && other.CompareTag("Blue Tower"));

            if (isFriendlyTower)
            {
                return; // 味方のタワーなので何もしない
            }

            // 敵のタワーだったので、そのタワーの TakeDamage を呼ぶ
            tower.TakeDamage(attackPower);
            Debug.Log(other.name + " にヒット！");
            Destroy(gameObject); // ヒットボックスを消滅させる
        }
    }
}