using UnityEngine;
using UnityEngine.UI;

public class BossManager : EnemyManager
{
    public const float MaxHp = 200f;
    public const float DefaultDamage = 30f;

    [SerializeField] GameObject _deathEffect;

    [SerializeField] GameObject _BossArea;
    [SerializeField] Slider _HpSlider;
    [SerializeField] GameObject _minionObject;

    public float CurrentHp { get; private set; }
    public Animator BossAnimator { get; private set; }

    private bool _isDead;
    private AIBossBrain _aiBrain;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        BossAnimator = GetComponent<Animator>();

        CurrentHp = MaxHp;

        _isDead = false;

        _direction = DirectionType.Left;
        _enemyType = EnemyType.Boss;

        _aiBrain = new AIBossBrain();

        switch (_gameManager.GameDifficulty)
        {
            case TitleData.Difficulty.Easy:
                _damageValue = DefaultDamage / 2;
                break;
            case TitleData.Difficulty.Normal:
                _damageValue = DefaultDamage;
                break;
            case TitleData.Difficulty.Difficult:
                _damageValue = DefaultDamage * 2;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        /* センサークラスからシンボル状態を更新 */
        /* AI_Sensorの値からAI_Brainのシンボル更新 */
        _aiBrain.UpdateSymbol(gameObject, transform.position);

        /* 選択するゴールの更新 */
        _aiBrain.UpdateRoute(gameObject);

        /* 選択されたステートによって行動を実施 */
        _aiBrain.ExecuteAction(gameObject);
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            return; /* 死んでいる場合は動かないようにする */
        }

        _HpSlider.gameObject.SetActive(_aiBrain.IsFoundPlayer());
        _HpSlider.value = CurrentHp / MaxHp; // HPバー更新

        /* AI_Sensorで各値を更新 */
        _aiBrain.UpdateSensor(gameObject, transform.position);

        switch (_direction)
        {
            case DirectionType.Stop:
                _speed = 0;
                break;
            case DirectionType.Right:
                _speed = 2f;
                transform.localScale = new Vector3(1, 1, 1); // キャラクターを右向きに変更
                break;
            case DirectionType.Left:
                _speed = -2f;
                transform.localScale = new Vector3(-1, 1, 1); // キャラクターを左向きに変更
                break;
        }
        // rigidbodyを使って速度計算。Y方向は重力制御があるのでそのままの値を入れる
        _rigidbody2D.velocity = new Vector2(_speed, _rigidbody2D.velocity.y);

        BossAnimator.SetFloat("speed", Mathf.Abs(_speed)); // アニメーションをRUNへ変更
    }

    /// <summary>
    /// メタAIへ通知
    /// </summary>
    private void OnEnable()
    {
        _initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z); /* 破壊された際にGameManagerが参照する用のパラメータ */
        _initialRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        GameObject managerObj = GameObject.Find("GameManager");
        if (managerObj != null)
        {
            _gameManager = managerObj.GetComponent<GameManager>();
        }
    }

    /// <summary>
    /// メタAIへ通知
    /// </summary>
    private void OnDisable()
    {
        if (_gameManager != null)
        {
            _gameManager.NotifyEnemyDeath(_enemyType, _initialPosition, _initialRotation);
        }
        else
        {
            Debug.Log(gameObject.name + "Destroy: no GameManager");
        }

    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("item"))
        {
            // アイテム取得
            CurrentHp += collision.gameObject.GetComponent<ItemManager>().AcquireItem();
            if (CurrentHp > MaxHp)
            {
                CurrentHp = MaxHp;
            }
        }

        _aiBrain.NotifyCollision(collision.gameObject);
    }

    public GameObject GetBossArea()
    {
        return _BossArea;
    }

    public bool Damaged(float attackDamage)
    {
        bool ret = false;
        CurrentHp -= attackDamage;
        
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            ret = true;
        }

        Debug.Log("HP:" + CurrentHp);

        return ret;
    }

    public void GenerateMinion()
    {
        
        if (transform.localScale.x > 0)
        {
            GameObject minion = Instantiate(_minionObject, new Vector3(transform.position.x + 0.1f, transform.position.y - 0.1f, 0), transform.rotation);
            minion.GetComponent<EnemyManager>().Type = EnemyType.Minion;
            minion.GetComponent<EnemyManager>().Direction = DirectionType.Right;
        }
        else
        {
            GameObject minion = Instantiate(_minionObject, new Vector3(transform.position.x - 0.1f, transform.position.y - 0.1f, 0), transform.rotation);
            minion.GetComponent<EnemyManager>().Type = EnemyType.Minion;
            minion.GetComponent<EnemyManager>().Direction = DirectionType.Left;
        }
    }

    public void Destroy()
    {
        _HpSlider.gameObject.SetActive(false);
        
        Instantiate(_deathEffect, transform.position, transform.rotation); // 死亡エフェクトを開始
        Destroy(gameObject);
    }
}
