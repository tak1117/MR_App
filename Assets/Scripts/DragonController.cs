using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("攻撃用の設定")]
    public GameObject attackHitboxPrefab;

    [Header("ステータス設定")]
    public int maxHp = 100;
    public int attackPower = 25;
    private int currentHp;

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

    private Transform target;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;

    private enum DragonState
    {
        Searching,
        Chasing,
        Attacking
    }
    private DragonState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        currentState = DragonState.Searching;
        lastAttackTime = -attackCooldown;
    }

    void Update()
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

    void FindOpponent()
    {
        float startAngle = -detectionAngle / 2f;
        float angleStep = detectionAngle / (numberOfRays - 1);

        Vector3 rayOrigin = transform.position;
        rayOrigin.y += 2f; 

        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;
            Debug.Log("索敵中");
            RaycastHit hit;
            
            // ▼▼▼ ご指定のロジックに修正 ▼▼▼
            if (Physics.Raycast(rayOrigin, direction, out hit, detectionRange))
            {
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red);
                Debug.Log("Rayが当たったオブジェクト: " + hit.collider.gameObject.name);
                // 当たった相手のタグが"Dragon"であり、かつ自分自身でなければ
                if (hit.collider.CompareTag("Dragon") && hit.transform != this.transform)
                {
                    Debug.Log("Dragonに当たりました！");
                    // 索敵完了の処理
                    target = hit.transform;
                    currentState = DragonState.Chasing;
                    return;
                }
            }
            // ▲▲▲ ご指定のロジックに修正 ▲▲▲
            else
            {
                Debug.DrawRay(rayOrigin, direction * detectionRange, Color.green);
            }
        }
    }

    void HandleChasing()
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

    void HandleAttacking()
    {
        if (target == null) return;
        
        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
            currentState = DragonState.Chasing;
        }
        
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackDistance)
        {
            currentState = DragonState.Chasing;
        }
    }

    private void PerformAttack()
    {
        animator.SetBool("IsMoving", false);
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
    }
    
    public void OnAttackAnimationStart()
    {
        LaunchAttack();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        Debug.Log(gameObject.name + " が " + damage + " ダメージを受けた！ 残りHP: " + currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " は倒れた...");
        animator.SetTrigger("Die");
        
        float dieAnimationLength = GetAnimationLength("Die");
        Destroy(gameObject, dieAnimationLength);
    }

    private float GetAnimationLength(string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        Debug.LogWarning("Animation clip '" + clipName + "' not found. Defaulting to 2 seconds.");
        return 2.0f;
    }

    public void LaunchAttack()
    {
        if (target == null || attackHitboxPrefab == null) return;
        GameObject hitboxObject = Instantiate(attackHitboxPrefab, target.position, target.rotation);
        HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();
        if (hitbox != null)
        {
            hitbox.attackPower = this.attackPower;
        }
        Destroy(hitboxObject, 0.5f);
    }
}