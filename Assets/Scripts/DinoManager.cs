using UnityEngine;


public class DinoManager : EnemyManager
{
    public const float DefaultDamage = 10f;
    [SerializeField] GameObject _deathEffect;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _direction = DirectionType.Right;
        _enemyType = EnemyType.Dino;

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

    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("trap"))
        {
            DestroyEnemy();
        }
    }
    public override void DestroyEnemy()
    {
        Instantiate(_deathEffect, transform.position, transform.rotation); // 死亡エフェクトを開始
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        switch (_direction)
        {
            case DirectionType.Stop:
                _speed = 0;
                break;
            case DirectionType.Right:
                _speed = 3;
                transform.localScale = new Vector3(-1, 1, 1); // キャラクターを右向きに変更
                break;
            case DirectionType.Left:
                _speed = -3;
                transform.localScale = new Vector3(1, 1, 1); // キャラクターを左向きに変更
                break;
        }
        // rigidbodyを使って速度計算。Y方向は重力制御があるのでそのままの値を入れる
        _rigidbody2D.velocity = new Vector2(_speed, _rigidbody2D.velocity.y);
    }
}

