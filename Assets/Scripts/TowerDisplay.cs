using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDisplay : MonoBehaviour
{
    [Header("HPバーの設定")]
    [SerializeField]
    private HPBarController hpBarController; // ドラゴンのものと同様にHPバーをインスペクタから設定

    [Header("タワーのステータス")]
    [Tooltip("このタワーの初期ヒットポイント（HP）を設定します。")]
    public float maxHp = 500f; // タワーなのでHPを高めに設定
    public float currentHp;

    void Start()
    {
        currentHp = maxHp;
    }
}