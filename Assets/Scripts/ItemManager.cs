using UnityEngine;

public abstract class ItemManager : MonoBehaviour
{
    public enum ItemType
    {
        Cherry,
        Gem
    }

    
    protected int _recoveryValue;

    protected GameManager _gameManager;

    protected Vector3 _initialPosition;
    protected Quaternion _initialRotation;


    public virtual int AcquireItem()
    {
        return _recoveryValue;
    }
}
