using UnityEngine;

public class AIActionApproach : AIActionState
{
    private int _keepStateCount;
    private const int MinionChangeInterval = 5;

    public AIActionApproach()
    {
        _tagStr = "approach";
        _keepStateCount = 0;
    }

    /// <summary>
    /// プレイヤーを追いかけ続ける
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Execute(AIBrainBase brain, GameObject targetObj)
    {
        EnemyManager boss = targetObj.GetComponent<EnemyManager>();
        if (boss != null)
        {
            var directionToEnemyVector = brain.GetAISensor().NeighborEnemyPosition - targetObj.transform.position;

            if (directionToEnemyVector.x > 0.01)
            {
                boss.Direction = EnemyManager.DirectionType.Right;
            }
            else if (directionToEnemyVector.x < 0.01)
            {
                boss.Direction = EnemyManager.DirectionType.Left;
            }
        }
    }

    /// <summary>
    /// このステートに入ったら一度進行方向をリセット
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Enter(AIBrainBase brain, GameObject targetObj)
    {
        Debug.Log(_tagStr);
        EnemyManager boss = targetObj.GetComponent<EnemyManager>();
        if (boss != null)
        {
            boss.Direction = EnemyManager.DirectionType.Stop;
        }
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
    /// 一定時間経過時はAIActionGenerateMinionへ変更。プレイヤーが攻撃範囲内に来たらAIActionAttack、プレイヤーが遠ざかったらAIActionWalkAroundへ変更
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public override (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        var symbol = brain.GetSymbolList();
        string nextStateStr = null;

        if (symbol.ContainsKey(AIBrainBase.StateSymbol.Attackable) && symbol.ContainsKey(AIBrainBase.StateSymbol.FoundPlayer))
        {
            if (symbol[AIBrainBase.StateSymbol.Attackable])
            {
                nextStateStr = "attack";
                return (true, nextStateStr);
            }
            else if (MinionChangeInterval / Time.deltaTime <= _keepStateCount)
            {
                nextStateStr = "generateMinion";
                _keepStateCount = 0;
                return (true, nextStateStr);
            }
            else if (!symbol[AIBrainBase.StateSymbol.FoundPlayer])
            {
                nextStateStr = "walkAround";
                return (true, nextStateStr);
            }
            else
            {
                _keepStateCount++;
            }
        }

        return (false, nextStateStr);
    }
}
