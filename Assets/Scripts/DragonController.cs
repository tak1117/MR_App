using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("移動と回転の設定")]
    public float moveSpeed = 2.0f;      // 移動速度
    public float rotationSpeed = 5.0f;  // 振り向く速度

    [Header("攻撃設定")]
    public float attackDistance = 5.0f; // この距離まで近づいたら攻撃する

    private Transform target;           // 攻撃対象のドラゴン
    private Animator animator;          // アニメーション制御用

    // ドラゴンの状態を定義
    private enum DragonState
    {
        Idle,       // 待機
        Searching,  // 索敵中
        Moving,     // 移動中
        Attacking   // 攻撃中
    }
    private DragonState currentState; // 現在の状態

    void Start()
    {
        // 自身のAnimatorコンポーネントを取得
        animator = GetComponent<Animator>();
        // 初期状態は索敵から
        currentState = DragonState.Searching;
    }

    void Update()
    {
        FindOpponent();
        // 状態に応じて処理を切り替え
        switch (currentState)
        {
            case DragonState.Idle:
                // 何もしない（または待機アニメーション）
                break;
            /*
            case DragonState.Searching:
                FindOpponent();
                break;
            */
            case DragonState.Moving:
                MoveTowardsTarget();
                break;
            case DragonState.Attacking:
                AttackTarget();
                break;
        }
    }

    // 敵を探す処理
    void FindOpponent()
    {
        // "Dragon"タグが付いている全てのゲームオブジェクトを検索
        GameObject[] dragons = GameObject.FindGameObjectsWithTag("Dragon");

        foreach (GameObject dragon in dragons)
        {
            // 見つかったドラゴンが自分自身でなければ、それをターゲットに設定
            if (dragon != this.gameObject)
            {
                target = dragon.transform;
                // ターゲットを見つけたら、移動状態に遷移
                currentState = DragonState.Moving;
                Debug.Log(this.name + "がターゲット(" + target.name + ")を発見！");
                return; // ターゲットが見つかったらループを抜ける
            }
        }
    }

    // ターゲットに向かって移動する処理
    void MoveTowardsTarget()
    {
        if (target == null)
        {
            // ターゲットを失ったら索敵状態に戻る
            currentState = DragonState.Searching;
            animator.SetBool("IsMoving", false); // 移動アニメーションを停止
            return;
        }

        // ターゲットとの距離を計算
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            // 攻撃範囲内に入ったら、攻撃状態に遷移
            currentState = DragonState.Attacking;
            animator.SetBool("IsMoving", false); // 移動アニメーションを停止
        }
        else
        {
            // ターゲットの方向を向く
            Vector3 direction = (target.position - transform.position).normalized;
            // Y軸の回転のみに限定して、急な傾きを防ぐ
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // ターゲットに向かって前進
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // 移動アニメーションを再生
            animator.SetBool("IsMoving", true);
        }
    }

    // 攻撃処理
    void AttackTarget()
    {
        if (target == null)
        {
             // 攻撃中にターゲットを失ったら索敵状態に戻る
            currentState = DragonState.Moving;
            return;
        }
        
        // 念のため、ターゲットの方向を向く
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;

        // 攻撃アニメーションを再生
        animator.SetTrigger("Attack");

        // 攻撃後、少し待ってから再度距離をチェックして移動状態に戻る
        // ここでは、一度攻撃したら再度距離をチェックするためにMoving状態に戻す
        // 連続攻撃したい場合は、このロジックを調整する
        StartCoroutine(AttackCooldown());
    }

    // 攻撃後のクールダウン
    IEnumerator AttackCooldown()
    {
        // 攻撃アニメーションの長さに合わせて待つ（例：2秒）
        yield return new WaitForSeconds(2.0f);
        // 状態を移動に戻して、再度距離を判定させる
        currentState = DragonState.Moving;
    }
}