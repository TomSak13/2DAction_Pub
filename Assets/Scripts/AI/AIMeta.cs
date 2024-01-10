using System.Collections.Generic;
using UnityEngine;
public class AIMeta
{
    private const int RespawnEnemyInterval = 100;
    private const int RespawnItemInterval = 300;
    private const int SpawnPlayerInterval = 10000;
    private class EnemyData
    {
        public EnemyManager.EnemyType Type;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    private class ItemData
    {
        public ItemManager.ItemType Type;
        public Vector3 Position;
        public Quaternion Rotation;
    }


    private float _playTime;
    private bool _isTimerStarted;
    private float _playTimeRespawnLimit;

    private float _enemySpawnTime;
    private bool _isEnemySpawnMeasured;

    private float _itemSpawnTime;
    private bool _isItemSpawnMeasured;

    private List<EnemyData> _spawnEnemies;
    private List<ItemData> _spawnItems;

    public AIMeta()
    {
        _playTime = 0;
        _isTimerStarted = false;
        _playTimeRespawnLimit = SpawnPlayerInterval;

        _enemySpawnTime = 0;
        _isEnemySpawnMeasured = false;

        _itemSpawnTime = 0;
        _isItemSpawnMeasured = false;

        _spawnEnemies = new List<EnemyData>();
        _spawnItems = new List<ItemData>();

    }

    public void RespawnEnemy(Dictionary<EnemyManager.EnemyType, GameObject> enemyDictionary)
    {
        EnemyData enemyData = _spawnEnemies[0];

        if (enemyDictionary[enemyData.Type] == null)
        {
            Debug.Log("AI_meta: EnemyRespawn no found enemy");
            return;
        }

        GameObject.Instantiate(enemyDictionary[enemyData.Type], enemyData.Position, enemyData.Rotation);

        _enemySpawnTime = 0;
        _spawnEnemies.RemoveAt(0);
        if (_spawnEnemies.Count == 0)
        {
            _isEnemySpawnMeasured = false;
        }
    }

    public void RespawnItem(Dictionary<ItemManager.ItemType, GameObject> itemDictionary)
    {
        ItemData itemData = _spawnItems[0];

        if (itemDictionary[itemData.Type] == null)
        {
            Debug.Log("AI_meta: EnemyRespawn no found item");
            return;
        }
        
        GameObject.Instantiate(itemDictionary[itemData.Type], itemData.Position, itemData.Rotation);

        _itemSpawnTime = 0;
        _spawnItems.RemoveAt(0);
        if (_spawnItems.Count == 0)
        {
            _isItemSpawnMeasured = false;
        }
    }

    public void TimeExpiredRespawnEnemy()
    {
        /* 画面外の左上の所から敵生成 */
        Vector3 respawnPosition = new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).x + 1,
                                                Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y - 1,
                                                0);

        EnemyData spawnEnemy = new EnemyData();
        spawnEnemy.Type = EnemyManager.EnemyType.MonsterDog;
        spawnEnemy.Position = respawnPosition;
        spawnEnemy.Rotation = new Quaternion(0, 0, 0, 1);

        _spawnEnemies.Add(spawnEnemy);

        /* 一度経過時間が一定時間を超えた場合には、次に敵を増やすまでのインターバルを縮める */
        /* 最初に敵を増やしたら、その次からはRESPAWN_ENEMY_INTERVALの値分のカウントが経過するたびに */
        /* 敵が増える */

        _isEnemySpawnMeasured = true;

        if (!float.IsInfinity(_playTimeRespawnLimit))
        {
            _playTimeRespawnLimit += RespawnEnemyInterval;
        }
    }

    public bool IsRespawnEnemy()
    {
        return _spawnEnemies.Count > 0 && _enemySpawnTime > RespawnEnemyInterval;
    }

    public bool IsRespawnItem()
    {
        return _spawnItems.Count > 0 && _itemSpawnTime > RespawnItemInterval;
    }

    public bool IsTimeExpiredRespawnEnemy()
    {
        return _playTime > _playTimeRespawnLimit;
    }

    /// <summary>
    /// GameManagerのupdate関数で呼ばれる。リスポーン管理
    /// </summary>
    /// <param name="enemyDictionary"></param>
    /// <param name="itemDictionary"></param>
    public void CheckSpawn(Dictionary<EnemyManager.EnemyType, GameObject> enemyDictionary,
        Dictionary<ItemManager.ItemType, GameObject> itemDictionary)
    {
        /* 経過時間が一定時間を超えると、敵を増やす */
        if (_playTime > _playTimeRespawnLimit)
        {
            TimeExpiredRespawnEnemy();
        }
        /* 倒した敵は一定期間が経過すると復活 */
        if (_spawnEnemies.Count > 0 && _enemySpawnTime > RespawnEnemyInterval)
        {
            RespawnEnemy(enemyDictionary);
        }
        /* 取得したアイテムも同様に復活させる */
        if (_spawnItems.Count > 0 && _itemSpawnTime > RespawnItemInterval)
        {
            RespawnItem(itemDictionary);
        }
    }

    /// <summary>
    /// GameManagerのFixedUpdateで呼ばれる。リスポーンの時間管理に使用
    /// </summary>
    public void UpdateTimer()
    {
        if (_isTimerStarted)
        {
            _playTime++;
        }

        if (_isEnemySpawnMeasured)
        {
            _enemySpawnTime++;
        }

        if (_isItemSpawnMeasured)
        {
            _itemSpawnTime++;
        }
    }

    public void StartTimer()
    {
        _isTimerStarted = true;
    }


    /// <summary>
    /// 敵とプレイヤーの接触時のコールバック
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="enemyPosition"></param>
    /// <param name="enemyRotation"></param>
    public void NotifyEnemyDeath(EnemyManager.EnemyType enemyType, Vector3 enemyPosition, Quaternion enemyRotation)
    {
        if (enemyType == EnemyManager.EnemyType.Minion)
        {
            return;
        }

        EnemyData spawnEnemy = new EnemyData
        {
            Type = enemyType,
            Position = enemyPosition,
            Rotation = enemyRotation
        };

        _isEnemySpawnMeasured = true;

        _spawnEnemies.Add(spawnEnemy);
    }

    /// <summary>
    /// アイテムの取得(接触)時のコールバック
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemPosition"></param>
    /// <param name="itemRotation"></param>
    public void NotifyItemUse(ItemManager.ItemType itemType, Vector3 itemPosition, Quaternion itemRotation)
    {
        ItemData spawnItem = new ItemData
        {
            Type = itemType,
            Position = itemPosition,
            Rotation = itemRotation
        };

        _isItemSpawnMeasured = true;

        _spawnItems.Add(spawnItem);
    }
}
