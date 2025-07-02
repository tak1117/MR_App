using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("攻撃用の設定")]
    public GameObject attackHitboxPrefab; // Inspectorから当たり判定プレハブを設定

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
    public float attackHitDelay = 0f;

    private Transform target;
    private Animator animator;
    private float lastAttackTime;

    // ドラゴンの状態を定義
    private enum DragonState
    {
        Searching,  // 索敵中
        Chasing,    // 追跡中 (移動)
        Attacking   // 攻撃中
    }
    private DragonState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHp = maxHp; // HPを最大値で初期化
        currentState = DragonState.Searching;
        lastAttackTime = -attackCooldown; // 最初から攻撃できるようにする
    }

    void Update()
    {
        // ターゲットがいなければ探す
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
                currentState = DragonState.Chasing; // 追跡状態に移行
                return;
            }
        }
    }

    void HandleChasing()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // 攻撃範囲に入ったら攻撃状態へ
        if (distance <= attackDistance)
        {
            currentState = DragonState.Attacking;
        }
        else // 範囲外なら追跡を続ける
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
        
        // 攻撃のクールダウンが終わっているかチェック
        if (Time.time > lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackCoroutine());
            currentState = DragonState.Chasing;
        }
        
        // 攻撃後、相手が範囲外に出たら追跡に戻る
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackDistance)
        {
            currentState = DragonState.Chasing;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // 攻撃状態の初期設定
        animator.SetBool("IsMoving", false);
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        lastAttackTime = Time.time;

        // 攻撃アニメーションを再生
        animator.SetTrigger("Attack");
        Debug.Log("Attack Animation Played !");

        yield return new WaitForSeconds(attackHitDelay);
        // 待った後に当たり判定を生成する
        LaunchAttack();
    }
    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        Debug.Log(gameObject.name + " が " + damage + " ダメージを受けた！ 残りHP: " + currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " は倒れた...");
        Destroy(gameObject, 2.0f);
    }

    public void LaunchAttack()
    {
        if (target == null || attackHitboxPrefab == null) return;

        // プレハブを「敵の位置」に生成する
        GameObject hitboxObject = Instantiate(attackHitboxPrefab, target.position, target.rotation);
        Debug.Log("Hit box appeared !");

        // 生成した当たり判定に攻撃力を設定する
        HitBoxController hitbox = hitboxObject.GetComponent<HitBoxController>();
        if (hitbox != null)
        {
            hitbox.attackPower = this.attackPower;
        }

        // 当たり判定が誰にも当たらなかった場合、0.5秒後に自動で消滅させる
        Destroy(hitboxObject, 0.5f);
    }
}