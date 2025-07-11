using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    // ★変更点: int から float へ
    public float attackPower = 0f;

    private void OnTriggerEnter(Collider other)
    {
        DragonBaseController target = other.GetComponent<DragonBaseController>();

        if (target != null)
        {
            target.TakeDamage(attackPower);
            Debug.Log(other.name + " にヒット！");
            Destroy(gameObject);
        }
    }
}