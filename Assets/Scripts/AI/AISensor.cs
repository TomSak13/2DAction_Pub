using System.Collections.Generic;
using UnityEngine;

public class AISensor
{
    public enum GameObjectTag
    {
        Player,
        Enemy,
        Boss,
        Item,
        BossBattle,
        Trap,
        Field,
        Confiner
    }

    private const float NeighborObjDistance = 10f;
    private const float DistanceInfiniteValue = 10000f;
    private const int MaxNeighborObj = 50;
    public float DistanceToEnemy { get; private set; }
    public Vector3 NeighborEnemyPosition { get; private set; }
    public Vector3 NeighborItemPosition { get; private set; }
    public float RatioHp { get; private set; }

    private RaycastHit2D[] _neighborObj;

    public AISensor()
    {
        DistanceToEnemy = DistanceInfiniteValue;
        RatioHp = 1f;

        _neighborObj = new RaycastHit2D[MaxNeighborObj];
    }

    
    /// <summary>
    /// 敵オブジェクトのupdate関数で呼ばれる。呼び出し元のオブジェクトに一番近いアイテムとプレイヤーの位置を更新する
    /// </summary>
    /// <param name="position"></param>
    /// <param name="enemyTag"></param>
    public void UpdateNeighborObj(Vector3 position, GameObjectTag enemyTag)
    {
        float distanceToEnemy = DistanceInfiniteValue;
        float distanceNeighborItem = DistanceInfiniteValue;
        Vector3 enemyPosition = Vector3.zero;
        Vector3 itemPosition = Vector3.zero;

        int arrayNum = Physics2D.CircleCastNonAlloc(
                position,
                NeighborObjDistance,/* 半径10以内を探す */
                Vector3.forward,
                _neighborObj);

        string enemyTagStr = System.Enum.GetName(typeof(GameObjectTag), enemyTag);
        if (enemyTagStr == null)
        {
            return; // tag設定ミス時はそのままリターン
        }

        for (int i = 0; i <arrayNum; i++)
        {
            var hit = _neighborObj[i];
            /* 一番近い敵をターゲットにする。アイテムも一番近いもの */
            if (hit.collider.gameObject.CompareTag(enemyTagStr.ToLower()))
            {
                if (distanceToEnemy > (hit.collider.gameObject.transform.position - position).sqrMagnitude)
                {
                    distanceToEnemy = (hit.collider.gameObject.transform.position - position).sqrMagnitude;
                    enemyPosition = hit.collider.gameObject.transform.position;
                }
            }
            else if (hit.collider.gameObject.CompareTag(nameof(GameObjectTag.Item).ToLower()))
            {
                if (distanceNeighborItem > (hit.collider.gameObject.transform.position - position).sqrMagnitude)
                {
                    distanceNeighborItem = (hit.collider.gameObject.transform.position - position).sqrMagnitude;
                    itemPosition = hit.collider.gameObject.transform.position;
                }
            }
        }

        NeighborEnemyPosition = enemyPosition;
        DistanceToEnemy = distanceToEnemy;
        
        NeighborItemPosition = itemPosition;
    }

    public void CalcHpRatio(float maxHp, float currentHp)
    {
        RatioHp = currentHp / maxHp;
    }
}
