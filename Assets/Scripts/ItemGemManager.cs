
using UnityEngine;

public class ItemGemManager : ItemManager
{
    private const int RecoveryGemVal = 60;

    private ItemType _type;

    private void Start()
    {
        _recoveryValue = RecoveryGemVal;
        _type = ItemType.Gem;
    }

    private void OnEnable()
    {
        Vector3 currentPosition = transform.position;
        Quaternion currentQuaternion = transform.rotation;
        _initialPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z); /* 破壊された際にGameManagerが参照する用のパラメータ */
        _initialRotation = new Quaternion(currentQuaternion.x, currentQuaternion.y, currentQuaternion.z, currentQuaternion.w);
        GameObject managerObj = GameObject.Find("GameManager");
        if (managerObj != null)
        {
            _gameManager = managerObj.GetComponent<GameManager>();
        }
        else
        {
            Debug.Log(gameObject.name + ":no GameManager");
        }
    }

    private void OnDisable()
    {
        if (_gameManager != null)
        {
            _gameManager.NotifyGetItem(_type, _initialPosition, _initialRotation);
        }
        else
        {
            Debug.Log(gameObject.name + ":no GameManager");
        }
    }

    override public int AcquireItem()
    {
        Destroy(this.gameObject);

        return _recoveryValue;
    }
}
