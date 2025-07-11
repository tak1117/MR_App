using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public float attackPower = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // 衝突した相手が DragonBaseController を持っているか試す
        if (other.TryGetComponent<DragonBaseController>(out var dragon))
        {
            // 持っていたら、そのドラゴンの TakeDamage を呼ぶ
            dragon.TakeDamage(attackPower);
            Debug.Log(other.name + " にヒット！");
            Destroy(gameObject); // ヒットボックスを消滅させる
        }
        else if (other.TryGetComponent<TowerHpController>(out var tower))
        {
            // 持っていたら、そのタワーの TakeDamage を呼ぶ
            tower.TakeDamage(attackPower);
            Debug.Log(other.name + " にヒット！");
            Destroy(gameObject); // ヒットボックスを消滅させる
        }
    }
}