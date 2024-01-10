using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum InvokeName
    {
        RestartScene,
    }

    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _gameClearText;

    // SE
    [SerializeField] private AudioClip _gameClearSE;
    private AudioSource _audioSource;

    private AIMeta _metaAI;

    [SerializeField] private GameObject _cherryItemObj;
    [SerializeField] private GameObject _gemItemOj;

    [SerializeField] private GameObject _opossumObj;
    [SerializeField] private GameObject _monsterDogObj;
    [SerializeField] private GameObject _dinoObj;

    [SerializeField] private CommonParam _commonParam;

    private Dictionary<EnemyManager.EnemyType,GameObject> _enemiesByEnemyType;
    private Dictionary<ItemManager.ItemType,GameObject> _itemsByItemType;

    private float _playerHp;
    private bool _isBossDead;

    public TitleData.Difficulty GameDifficulty { get => _gameDifficulty; set => _gameDifficulty = value; }

    private TitleData.Difficulty _gameDifficulty;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _metaAI = new AIMeta();

        _enemiesByEnemyType = new Dictionary<EnemyManager.EnemyType, GameObject>
        {
            { EnemyManager.EnemyType.Opossum, _opossumObj },
            { EnemyManager.EnemyType.MonsterDog, _monsterDogObj },
            { EnemyManager.EnemyType.Dino, _dinoObj }
        };

        _itemsByItemType = new Dictionary<ItemManager.ItemType, GameObject>
        {
            { ItemManager.ItemType.Cherry, _cherryItemObj },
            { ItemManager.ItemType.Gem, _gemItemOj }
        };

        _playerHp = PlayerManager.PlayerMaxHp;
        _isBossDead = false;

        _gameDifficulty = _commonParam.GameDifficulty;
        Debug.Log("難易度："+ _gameDifficulty);

        _metaAI.StartTimer();
    }

    private void Update()
    {
        if (_playerHp <= 0)
        {
            GameOver();
        }
        else if (_isBossDead)
        {
            GameClear();
        }
        else
        {
            /* 経過時間が一定時間を超えると、敵を増やす */
            if (_metaAI.IsTimeExpiredRespawnEnemy())
            {
                _metaAI.TimeExpiredRespawnEnemy();
            }
            /* 倒した敵は一定期間が経過すると復活 */
            if (_metaAI.IsRespawnEnemy())
            {
                _metaAI.RespawnEnemy(_enemiesByEnemyType);
            }
            /* 取得したアイテムも同様に復活させる */
            if (_metaAI.IsRespawnItem())
            {
                _metaAI.RespawnItem(_itemsByItemType);
            }
        }
    }

    private void FixedUpdate()
    {
        _metaAI.UpdateTimer();
    }

    public void NotifyEnemyDeath(EnemyManager.EnemyType enemyType, Vector3 enemyPosition, Quaternion enemyRotation)
    {
        if (enemyType == EnemyManager.EnemyType.Boss)
        {
            _isBossDead = true;
        }
        else
        {
            _metaAI.NotifyEnemyDeath(enemyType, enemyPosition, enemyRotation);
        }
    }

    public void NotifyGetItem(ItemManager.ItemType itemType, Vector3 itemPosition, Quaternion itemRotation)
    {
        if (_metaAI != null)
        {
            _metaAI.NotifyItemUse(itemType, itemPosition, itemRotation);
        }
    }

    public void GetPlayerHp(float playerHp)
    {
        _playerHp = playerHp;
    }

    public void GameOver()
    {
        _gameOverText.SetActive(true);
        Time.timeScale = 0; // 停止処理
        Time.timeScale = 1; // 停止解除
        Invoke(nameof(InvokeName.RestartScene), 1.5f);
    }

    public void GameClear()
    {
        _gameClearText.SetActive(true);
        _audioSource.PlayOneShot(_gameClearSE);
        Invoke(nameof(InvokeName.RestartScene), 1.5f); // 1.5秒後にRestartSceneを実行

        _isBossDead = false;
    }

    /// <summary>
    /// ゲームリスタート
    /// </summary>
    public void RestartScene()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name); // 今のシーンをもう一度読み込むことでリスタートする
    }
}
