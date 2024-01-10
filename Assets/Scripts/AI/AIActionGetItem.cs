using UnityEngine;

public class AIActionGetItem : AIActionState
{
    private bool _isGetItem;
    public AIActionGetItem()
    {
        _tagStr = "getItem";
        _isGetItem = false;
    }
    
    /// <summary>
    /// 一番近くにあるアイテムのところへ移動する
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Execute(AIBrainBase brain, GameObject targetObj)
    {
        if (_isGetItem)
        {
            return; /* すでにアイテムを使用している場合はこのステートに入らないようにする */
        }
        EnemyManager boss = targetObj.GetComponent<EnemyManager>();

        if (boss != null)
        {
            Vector2 directionToItemVector = brain.GetAISensor().NeighborItemPosition - boss.transform.position;

            if ((int)directionToItemVector.x > 0)
            {
                boss.Direction = EnemyManager.DirectionType.Right;
            }
            else if ((int)directionToItemVector.x < 0)
            {
                boss.Direction = EnemyManager.DirectionType.Left;
            }
        }
    }

    /// <summary>
    /// このステートへ変更時は何もしない
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Enter(AIBrainBase brain, GameObject targetObj)
    {
        Debug.Log(_tagStr);
    }

    /// <summary>
    /// 別のステートへ変更時は何もしない
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Exit(AIBrainBase brain, GameObject targetObj)
    {
        
    }

    /// <summary>
    /// アイテムを取得した後はAIActionApproachに変更
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public override (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        bool ret = false;
        string nextStateStr = null;

        Debug.Log("stateChange GetItem");

        if (_isGetItem)
        {
            nextStateStr = new string("approach");
            ret = true;
            brain.SetSymbol(AIBrainBase.StateSymbol.ItemUsed, true);
        }

        return (ret, nextStateStr);
    }

    /// <summary>
    /// オブジェクトと接触した際に通知されるコールバック
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="collisionObj"></param>
    public override void NotifyCollision(AIBrainBase brain, GameObject collisionObj)
    {
        if (collisionObj == null)
        {
            return;
        }

        if (collisionObj.CompareTag("item"))
        {
            _isGetItem = true;
        }
    }
}
