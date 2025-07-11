using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public abstract class DragonBaseController : MonoBehaviour
{
    [Header("HPバーの設定")]
    [SerializeField]
    private HPBarController hpBarController;
    [Header("攻撃用の設定")]
    public GameObject attackHitboxPrefab;

    [Header("ステータス設定")]
    public float maxHp = 100f;
    public float attackPower = 25f;
    protected float currentHp;

    [Header("移動と回転の設定")]
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 5.0f;

    [Header("索敵・攻撃距離設定")]
    public float attackDistance = 5.0f;
    public float attackCooldown = 2.0f;
    [Header("レーダー索敵の設定")]
    public float detectionRange = 500f;
    public float detectionAngle = 60f;
    public int numberOfRays = 11;

    protected Transform target;
    protected Transform destination;
    protected Transform attackTarget;
    protected Animator animator;
    protected float lastAttackTime;
    protected bool isDead = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        lastAttackTime = -attackCooldown;

        if (this.CompareTag("Red Dragon"))
        {
            GameObject blueTower = GameObject.FindWithTag("Blue Tower");
            if (blueTower != null) destination = blueTower.transform;
        }
        else if (this.CompareTag("Blue Dragon"))
        {
            GameObject redTower = GameObject.FindWithTag("Red Tower");
            if (redTower != null) destination = redTower.transform;
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        FindOpponent();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position); // ★名称変更

            if (distanceToTarget <= attackDistance) // ★名称変更
            {
                attackTarget = target;
                HandleAttacking();
            }
            else
            {
                attackTarget = null;
                MoveTowards(target);
            }
        }
        else
        {
            if(destination != null)
            {
                float distanceToDestination = Vector3.Distance(transform.position, destination.position);
                if(distanceToDestination <= attackDistance)
                {
                    attackTarget = destination;
                    HandleAttacking();
                }
                else
                {
                    attackTarget = null;
                    MoveTowards(destination);
                }
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }

        if (hpBarController != null)
        {
            hpBarController.UpdateHP(currentHp, maxHp);
        }
    }

    protected virtual void FindOpponent()
    {
        float startAngle = -detectionAngle / 2f;
        float angleStep = detectionAngle / (numberOfRays > 1 ? numberOfRays - 1 : 1);
        Vector3 rayOrigin = transform.position + Vector3.up * 2f;

        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;
            RaycastHit hit;


            if (Physics.Raycast(rayOrigin, direction, out hit, detectionRange))
            {
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red);
                switch (this.tag)
                {
                    case "Red Dragon":
                        if(hit.collider.CompareTag("Blue Dragon")) target = hit.transform;
                        break;
                    case "Blue Dragon":
                        if (hit.collider.CompareTag("Red Dragon")) target = hit.transform;
                        break;
                        
                }
            }
            else
            {
                Debug.DrawRay(rayOrigin, direction * detectionRange, Color.green);
            }
        }
    }

    protected void MoveTowards(Transform moveTarget)
    {
        if (moveTarget == null)
        {
            animator.SetBool("IsMoving", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, moveTarget.position);

        if (distance > 2.0f)
        {
            animator.SetBool("IsMoving", true);
            Vector3 direction = (moveTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    protected virtual void HandleAttacking()
    {
        if (attackTarget == null) return;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    protected virtual void PerformAttack()
    {
        animator.SetBool("IsMoving", false);
        Vector3 direction = (attackTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHp -= damage;
        Debug.Log(gameObject.name + " が " + damage + " ダメージを受けた！ 残りHP: " + currentHp);
        if (hpBarController != null)
        {
            hpBarController.UpdateHP(currentHp, maxHp);
        }
        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log(gameObject.name + " は倒れた...");
        animator.SetTrigger("Die");
        float dieAnimationLength = GetAnimationLength("Die");
        Destroy(gameObject, dieAnimationLength);
    }

    protected float GetAnimationLength(string clipName)
    {
        if (animator.runtimeAnimatorController == null) return 2.0f;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Equals(clipName, System.StringComparison.OrdinalIgnoreCase))
            {
                return clip.length;
            }
        }
        Debug.LogWarning("Animation clip '" + clipName + "' not found. Defaulting to 2 seconds.");
        return 2.0f;
    }

    public virtual IEnumerator LaunchAttack()
    {
        if (attackTarget == null || attackHitboxPrefab == null) yield break;

        GameObject hitboxObject = Instantiate(attackHitboxPrefab, attackTarget.position, target.rotation);

        HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();
        if (hitbox != null)
        {
            hitbox.attackPower = this.attackPower;
        }
        Destroy(hitboxObject, 0.5f);
        yield return null;
    }

    public void OnAttackAnimationStart()
    {
        StartCoroutine(LaunchAttack());
    }
}