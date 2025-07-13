// using UnityEngine;

// public class HitBoxController : MonoBehaviour
// {
//     public float attackPower = 0f;
//     public string attackerTag; // ★追加：攻撃者のタグを保存する変数

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Box"))
//         {
//             return;
//         }

//         // 衝突した相手が DragonBaseController を持っているか試す
//         if (other.TryGetComponent<DragonBaseController>(out var dragon))
//         {
//             // ★追加：当たったドラゴンが味方なら、何もしない
//             if (other.CompareTag(attackerTag))
//             {
//                 return;
//             }

//             // 敵だったので、そのドラゴンの TakeDamage を呼ぶ
//             dragon.TakeDamage(attackPower);
//             Debug.Log(other.name + " にヒット！");
//             Destroy(gameObject); // ヒットボックスを消滅させる
//         }
//         // 衝突した相手が TowerHpController を持っているか試す
//         else if (other.TryGetComponent<TowerHpController>(out var tower))
//         {
//             // ★追加：当たったタワーが味方かどうかを判定
//             bool isFriendlyTower = (attackerTag.Contains("Red") && other.CompareTag("Red Tower")) ||
//                                  (attackerTag.Contains("Blue") && other.CompareTag("Blue Tower"));

//             if (isFriendlyTower)
//             {
//                 return; // 味方のタワーなので何もしない
//             }

//             // 敵のタワーだったので、そのタワーの TakeDamage を呼ぶ
//             tower.TakeDamage(attackPower);
//             Debug.Log(other.name + " にヒット！");
//             Destroy(gameObject); // ヒットボックスを消滅させる
//         }
//     }
// }

using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public float attackPower = 0f;
    public string attackerTag;

    private void OnTriggerEnter(Collider other)
    {
        // --- デバッグログ①：何に衝突したかを確認 ---
        Debug.Log($"[ヒットボックス] 攻撃者'{attackerTag}'の攻撃が、'{other.name}' (タグ: '{other.tag}') に衝突しました。");

        // 相手がドラゴンかタワーでなければ、処理を中断
        bool isTargetDragon = other.TryGetComponent<DragonBaseController>(out var dragon);
        bool isTargetTower = other.TryGetComponent<TowerHpController>(out var tower);

        if (!isTargetDragon && !isTargetTower)
        {
            Debug.LogWarning($"[ヒットボックス] 衝突相手'{other.name}'はドラゴンでもタワーでもないため、処理を中断します。");
            return;
        }

        // --- この時点で、ドラゴンかタワーのどちらかに当たったことが確定 ---

        // 1. 敵かどうかを判定する
        bool isEnemy =
            (attackerTag.Contains("Red") && other.tag.Contains("Blue")) ||
            (attackerTag.Contains("Blue") && other.tag.Contains("Red"));

        // --- デバッグログ②：敵として判定されたかを確認 ---
        Debug.Log($"[ヒットボックス] 敵として判定されましたか？ -> {isEnemy}");

        // 2. もし敵なら、ダメージを与える
        if (isEnemy)
        {
            Debug.Log($"[ヒットボックス] {other.name}にダメージを与えます！");
            if (isTargetDragon)
            {
                dragon.TakeDamage(attackPower);
            }
            else // isTargetTower
            {
                tower.TakeDamage(attackPower);
            }
        }
        else
        {
            Debug.LogWarning($"[ヒットボックス] 味方、または判定不能なターゲットのため、ダメージを与えません。");
        }

        // 3. 相手が敵でも味方でも、ユニットに当たった時点でヒットボックスは消滅させる
        Destroy(gameObject);
    }
}