using UnityEngine;


public abstract class EnemyManager : MonoBehaviour
{
    public enum DirectionType
    {
        Stop,
        Right,
        Left,
    }

    public enum EnemyType
    {
        Opossum,
        MonsterDog,
        Dino,
        Minion,
        Boss,
    }

    protected DirectionType _direction;
    protected Rigidbody2D _rigidbody2D;
    [SerializeField] protected LayerMask _blockLayer;
    protected GameManager _gameManager;
    protected float _speed;

    protected float _damageValue;

    protected EnemyType _enemyType;

    protected Vector3 _initialPosition;
    protected Quaternion _initialRotation;


    public virtual void DestroyEnemy()
    {

    }

    public float DamageValue
    {
        get => _damageValue;
        set => _damageValue = value;
    }

    public EnemyType Type
    {
        get => _enemyType;
        set => _enemyType = value;
    }

    public DirectionType Direction
    {
        get => _direction;
        set => _direction = value;
    }
}

