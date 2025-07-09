using UnityEngine;
using System.Collections;

// abstract: このスクリプト自体は直接オブジェクトにアタッチできない（親専用）という意味
public abstract class DragonBaseController : MonoBehaviour
{
    [Header("HPバーの設定")]
    [SerializeField]
    private HPBarController hpBarController;
    [Header("攻撃用の設定")]
    public GameObject attackHitboxPrefab;

    [Header("ステータス設定")]
    public int maxHp = 100;
    public int attackPower = 25;
    protected int currentHp;

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
    protected Animator animator;
    protected float lastAttackTime;
    protected bool isDead = false;

    protected enum DragonState
    {
        Searching,
        Chasing,
        Attacking
    }
    protected DragonState currentState;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        currentState = DragonState.Searching;
        lastAttackTime = -attackCooldown;

        if (hpBarController != null)
        {
            hpBarController.UpdateHP(currentHp, maxHp);
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (target == null)
        {
            currentState = DragonState.Searching;
        }

        switch (currentState)
        {
            case DragonState.Searching:
                animator.SetBool("IsMoving", false);
                FindOpponent();
                break;
            case DragonState.Chasing:
                HandleChasing();
                break;
            case DragonState.Attacking:
                HandleAttacking();
                break;
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
                if (hit.collider.CompareTag("Dragon") && hit.transform != this.transform)
                {
                    target = hit.transform;
                    currentState = DragonState.Chasing;
                    return;
                }
            }
            else
            {
                Debug.DrawRay(rayOrigin, direction * detectionRange, Color.green);
            }
        }
    }

    protected virtual void HandleChasing()
    {
        if (target == null) return;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackDistance)
        {
            currentState = DragonState.Attacking;
        }
        else
        {
            animator.SetBool("IsMoving", true);
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    protected virtual void HandleAttacking()
    {
        if (target == null) return;
        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackDistance)
        {
            currentState = DragonState.Chasing;
        }
    }

    protected virtual void PerformAttack()
    {
        animator.SetBool("IsMoving", false);
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(int damage)
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

    // virtual: 子クラスでこのメソッドの挙動を上書き（オーバーライド）できるようにする
    public virtual IEnumerator LaunchAttack()
    {
        if (target == null || attackHitboxPrefab == null) yield break;
        GameObject hitboxObject = Instantiate(attackHitboxPrefab, target.position, target.rotation);
        HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();
        if (hitbox != null)
        {
            hitbox.attackPower = this.attackPower;
        }
        Destroy(hitboxObject, 0.5f);
        yield return null;
    }

    // アニメーションイベントから呼ばれる関数
    public void OnAttackAnimationStart()
    {
        StartCoroutine(LaunchAttack());
    }
}