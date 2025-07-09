using UnityEngine;
using System.Collections;

// DragonBaseControllerを継承
public class UsurperDragonController : DragonBaseController
{
    // 攻撃方法(LaunchAttack)をこのドラゴン専用のものに上書き（オーバーライド）する
    public override IEnumerator LaunchAttack()
    {
        Debug.Log(this.name + " が独自の扇形攻撃を実行！");

        if (target == null || attackHitboxPrefab == null) yield break;

        float distance = Vector3.Distance(transform.position, target.position);
        float startAngle = -30f;
        float finishAngle = 30f;
        float time = 0f;
        float duration = 0.5f; // 扇形攻撃の持続時間

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            float currentAngle = Mathf.Lerp(startAngle, finishAngle, progress);

            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;
            Vector3 point = transform.position + direction * distance;

            GameObject hitboxObject = Instantiate(attackHitboxPrefab, point, Quaternion.identity);
            HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();

            if (hitbox != null)
            {
                hitbox.attackPower = this.attackPower;
            }

            Destroy(hitboxObject, 0.1f);

            yield return null;
        }
    }
}