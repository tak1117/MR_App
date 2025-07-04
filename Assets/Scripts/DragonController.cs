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

    private Transform target;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false; // 死亡状態を管理するフラグ

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
        // 死亡している場合は、以降の処理をすべて中断する
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
        GameObject[] dragons = GameObject.FindGameObjectsWithTag("Dragon");
        foreach (GameObject dragon in dragons)
        {
            if (dragon != this.gameObject)
            {
                target = dragon.transform;
                currentState = DragonState.Chasing;
                return;
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
        // 死亡している場合はダメージを受けない
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
        // 既に死亡処理が始まっている場合は何もしない
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " は倒れた...");
        
        // "Die"という名前のトリガーをAnimatorに送る
        animator.SetTrigger("Die");
        
        // アニメーションの長さを取得して、その時間後にオブジェクトを破壊する
        float dieAnimationLength = GetAnimationLength("Die");
        Destroy(gameObject, dieAnimationLength);
    }

    // 指定された名前のアニメーションクリップの長さを取得するヘルパー関数
    private float GetAnimationLength(string clipName)
    {
        // アニメーターに設定されているすべてのアニメーションクリップを調べる
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        // 見つからなかった場合はデフォルト値として2秒を返す
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