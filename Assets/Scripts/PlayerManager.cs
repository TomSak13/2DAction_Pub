using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public enum DirectionType
    {
        Stop,
        Right,
        Left,
    }

    public enum ActionState
    {
        None,
        Run,
        Attack,
        Damaged
    }
    public const float PlayerMaxHp = 100f;

    private const float PlayerJumpPower = 250f;
    private const float PlayerEnemyDestroyJumpPower = 400f;
    private const float PlayerDefaultAttackDamage = 30f;

    private static readonly int _speedAnimatorHash = Animator.StringToHash("speed");
    private static readonly int _isJumpingAnimatorHash = Animator.StringToHash("isJumping");
    private static readonly int _damagedAnimatorHash = Animator.StringToHash("Damaged");
    private static readonly int _deathAnimatorHash = Animator.StringToHash("PlayerDeathAnimation");

    [SerializeField] private LayerMask _blockLayer;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioClip _getItemSE;
    [SerializeField] private AudioClip _jumpSE;
    [SerializeField] private AudioClip _stampSE;
    [SerializeField] private AudioClip _deathSE;
    [SerializeField] private Slider _hpSlider;

    private DirectionType _direction;
    private Rigidbody2D _rigidbody2D;
    private float _speed;
    private float _jumpPower;
    
    private float _maxHp;
    private float _currentHp;
    private float _attackDamage;
    private ActionState _actionState;

    private Animator _actionAnimator;
    private AudioSource _bgmSource;

    private int _intervalChangeState;

    private float _jumpTimer;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _actionAnimator = GetComponent<Animator>();
        _bgmSource = GetComponent<AudioSource>();

        _maxHp = PlayerMaxHp;
        _currentHp = _maxHp;

        _direction = DirectionType.Stop;

        _actionState = ActionState.None;
        _intervalChangeState = 0;

        _jumpPower = PlayerJumpPower;
        _maxHp = PlayerMaxHp;
        _attackDamage = PlayerDefaultAttackDamage;

        _jumpTimer = 0;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal"); // 方向キーの取得
        _actionAnimator.SetFloat(_speedAnimatorHash, Mathf.Abs(x)); // アニメーションをRUNへ変更

        if (x == 0 && IsGround())
        {
            // 停止
            _direction = DirectionType.Stop;
        }
        else if (x > 0)
        {
            // 右へ
            _direction = DirectionType.Right; 
        }
        else if(x < 0)
        {
            // 左へ
            _direction = DirectionType.Left;
        }

        // スペースが押されたらジャンプ
        if (IsGround() && Input.GetKeyDown("space")) // キーを押して離した時に検知
        {
            Jump();
            _jumpTimer = 0.1f / Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (_currentHp <= 0)
        {
            return; /* 死んでいる場合は動かないようにする */
        }

        _hpSlider.value = _currentHp / _maxHp; // HPバー更新

        if (_actionState < ActionState.Attack)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                _actionAnimator.SetBool(_isJumpingAnimatorHash, false);
                _jumpTimer = 0;
            }
            else if (_jumpTimer > 0)
            {
                /* 長押しでジャンプ距離が延びる設定 */
                _jumpTimer--;
                if (_rigidbody2D.velocity.y < 5)
                {
                    _rigidbody2D.AddForce(Vector2.up * _jumpTimer);
                }
            }

            switch (_direction)
            {
                case DirectionType.Stop:
                    _actionState = ActionState.None;
                    _speed = 0;
                    break;
                case DirectionType.Right:
                    _actionState = ActionState.Run;
                    _speed = 3;
                    transform.localScale = new Vector3(1, 1, 1); // キャラクターを右向きに変更
                    break;
                case DirectionType.Left:
                    _actionState = ActionState.Run;
                    _speed = -3;
                    transform.localScale = new Vector3(-1, 1, 1); // キャラクターを左向きに変更
                    break;
            }
            /* rigidbodyを使って速度計算。Y方向は重力制御があるのでそのままの値を入れる */
            _rigidbody2D.velocity = new Vector2(_speed, _rigidbody2D.velocity.y);
        }
        else if (_actionState == ActionState.Damaged)
        {
            /* 0.75秒入力を無視 */
            _intervalChangeState++;
            if (_intervalChangeState >= (0.75 / Time.deltaTime))
            {
                _intervalChangeState = 0;
                _actionState = ActionState.None;
            }
        }
    }

    private void Jump()
    {
        _bgmSource.PlayOneShot(_jumpSE);
        // 上方向へ力を加える
        _rigidbody2D.AddForce(Vector2.up * _jumpPower);
        _actionAnimator.SetBool(_isJumpingAnimatorHash, true); // アニメーションをRUNへ変更
    }

    private void EnemydestroyJump()
    {
        _bgmSource.PlayOneShot(_jumpSE);
        // 上方向へ力を加える
        if (_rigidbody2D.velocity.y < 20)
        {
            _rigidbody2D.AddForce(Vector2.up * PlayerEnemyDestroyJumpPower);
        }
        _actionAnimator.SetBool(_isJumpingAnimatorHash, true); // アニメーションをRUNへ変更
    }

    private bool IsGround()
    {
        // ベクトルの始点と終点を作成(キャラクターから下へベクトルを作成し、直線がグラウンドについているかで判断)
        Vector3 leftStartPoint = transform.position - Vector3.right * 0.2f;
        Vector3 rightStartPoint = transform.position + Vector3.right * 0.2f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f;

        // 作成した線がオブジェクトに設置していると、TRUEを返す
        return Physics2D.Linecast(leftStartPoint, endPoint, _blockLayer) || Physics2D.Linecast(rightStartPoint, endPoint, _blockLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(nameof(AISensor.GameObjectTag.Trap).ToLower())) // tagに「trap」があるものにぶつかった場合の処理
        {
            OnEnterTrap();
        }
        else if (collision.gameObject.CompareTag(nameof(AISensor.GameObjectTag.Item).ToLower()))
        {
            OnEnterItem(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(nameof(AISensor.GameObjectTag.Enemy).ToLower()))
        {
            OnEnterEnemy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(nameof(AISensor.GameObjectTag.Boss).ToLower()))
        {
            OnEnterBoss(collision.gameObject);
        }
    }

    private void OnEnterTrap()
    {
        Death();
    }

    private void OnEnterItem(GameObject collisionItem)
    {
        // アイテム取得
        // 接触したオブジェクト側が持っているクラスへアクセスする
        _bgmSource.PlayOneShot(_getItemSE);
        _currentHp += collisionItem.GetComponent<ItemManager>().AcquireItem();
        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
    }

    private void OnEnterEnemy(GameObject collisionEnemy)
    {
        // 正面か上部かで分岐させる
        EnemyManager enemy = collisionEnemy.GetComponent<EnemyManager>();

        if (this.transform.position.y + 0.2f > enemy.transform.position.y)
        {
            // 上から踏んだ場合
            _bgmSource.PlayOneShot(_stampSE);
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0); // y方向のベクトルを削除(落ちてくる方向に速度があるためそれを削除)
            EnemydestroyJump();
            enemy.DestroyEnemy();
        }
        else
        {
            if (_currentHp - enemy.DamageValue > 0)
            {
                _currentHp -= enemy.DamageValue;
                PlayerDamagedAnimation(collisionEnemy);
            }
            else
            {
                Death();
            }
        }
    }

    private void OnEnterBoss(GameObject collisionBoss)
    {
        BossManager boss = collisionBoss.GetComponent<BossManager>();
        if (this.transform.position.y + 0.2f > boss.transform.position.y)
        {
            // 上から踏んだ場合
            _bgmSource.PlayOneShot(_stampSE);
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0); // y方向のベクトルを削除(落ちてくる方向に速度があるためそれを削除)
            EnemydestroyJump();
            if (boss.Damaged(_attackDamage))
            {
                boss.Destroy();
            }
        }
        else
        {
            if ((_currentHp + boss.DamageValue) > 0)
            {
                _currentHp -= boss.DamageValue;
                PlayerDamagedAnimation(collisionBoss);
            }
            else
            {
                Death();
            }
        }
    }

    private void PlayerDamagedAnimation(GameObject enemyObject)
    {
        _rigidbody2D.velocity = new Vector2(0, 0);
        if ((enemyObject.transform.position.x - transform.position.x) > 0)
        {
            _rigidbody2D.AddForce(new Vector2(-PlayerEnemyDestroyJumpPower, PlayerEnemyDestroyJumpPower / 2));
        }
        else
        {
            _rigidbody2D.AddForce(new Vector2(PlayerEnemyDestroyJumpPower, PlayerEnemyDestroyJumpPower / 2));
        }

        _actionState = ActionState.Damaged;
        _actionAnimator.SetTrigger(_damagedAnimatorHash);
        _hpSlider.GetComponent<PlayerHPSliderAnimation>().Blink();
    }

    private void Death()
    {
        _bgmSource.PlayOneShot(_deathSE);
        _rigidbody2D.velocity = new Vector2(0, 0);
        _rigidbody2D.AddForce(Vector2.up * 200);
        _actionAnimator.Play(_deathAnimatorHash);

        _currentHp = 0;
        _gameManager.GetPlayerHp(_currentHp);
        /* ステージに接触しないようにする */
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Destroy(boxCollider2D);
    }
}
