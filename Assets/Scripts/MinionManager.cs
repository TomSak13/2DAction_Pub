using UnityEngine;


public class MinionManager : EnemyManager
{
    public const float DefaultDamage = 10f;
    [SerializeField] GameObject _deathEffect;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _enemyType = EnemyType.Minion;
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
        if (IsHitWall())
        {
            DestroyEnemy();
        }
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
                transform.localScale = new Vector3(1, 1, 1); 
                break;
            case DirectionType.Left:
                _speed = -3;
                transform.localScale = new Vector3(-1, 1, 1); 
                break;
        }
        
        GetComponent<Rigidbody2D>().velocity = new Vector2(_speed, GetComponent<Rigidbody2D>().velocity.y);
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

    public bool IsHitWall()
    {
        Vector3 currentPosition = transform.position;
        Vector3 startPoint = currentPosition - (Vector3.up * 0.1f) + (Vector3.left * 0.2f);
        Vector3 endPoint = currentPosition - (Vector3.up * 0.1f) - (Vector3.left * 0.2f);

        return Physics2D.Linecast(startPoint, endPoint, _blockLayer);
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
}

