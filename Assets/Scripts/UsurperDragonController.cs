using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

// DragonBaseControllerを継承
public class UsurperDragonController : DragonBaseController
{
    [Header("扇形攻撃の設定")]
    [SerializeField] private GameObject FireObject; // 炎のエフェクト
    [SerializeField] private float attackDuration = 1.333f; // 攻撃の持続時間
    [SerializeField] private float startAngle = 30f;   // 開始角度
    [SerializeField] private float finishAngle = -30f; // 終了角度
    [SerializeField] private float spawnInterval = 0.1f; // 生成間隔

    // 攻撃メソッド(LaunchAttack)を、このドラゴン用に上書き（オーバーライド）する
    public override IEnumerator LaunchAttack()
    {
        Debug.Log(this.name + " が独自の扇形攻撃を実行！");

        if (target == null || attackHitboxPrefab == null) yield break;

        float distance = Vector3.Distance(transform.position, target.position);
        float time = 0f;

        while (time < attackDuration)
        {
            float progress = time / attackDuration;
            float currentAngle = Mathf.Lerp(startAngle, finishAngle, progress);

            // 攻撃地点の計算
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;
            Vector3 point = transform.position + direction * distance;

            // ヒットボックスとエフェクトを生成
            GameObject hitboxObject = Instantiate(attackHitboxPrefab, point, Quaternion.identity);
            GameObject fire = Instantiate(FireObject, point, Quaternion.identity);

            HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();

            if (hitbox != null)
            {
                // ★★【重要】攻撃力と攻撃者タグを設定する ★★
                hitbox.attackPower = this.attackPower;
                hitbox.attackerTag = this.gameObject.tag; // ← この行が不足していました
            }

            // 生成したオブジェクトを0.5秒後に破棄
            Destroy(hitboxObject, 0.5f);
            Destroy(fire, 0.5f);

            // 指定した間隔だけ待機
            yield return new WaitForSeconds(spawnInterval);

            time += spawnInterval;
        }
    }
}