using System.Collections.Generic;
using UnityEngine;

public class AIActionWalkAround : AIActionState
{
    
    public AIActionWalkAround()
    {
        _tagStr = "walkAround";
    }

    /// <summary>
    /// battleAreaの範囲内を歩き回る
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Execute(AIBrainBase brain, GameObject targetObj)
    {
        BossManager boss = targetObj.GetComponent<BossManager>();
        if (boss != null)
        {
            bool isArea = targetObj.GetComponent<Collider2D>().IsTouching(boss.GetBossArea().GetComponent<Collider2D>());

            /* エリア内を歩き回る */
            if (!isArea)
            {
                if (boss.Direction == EnemyManager.DirectionType.Left)
                {
                    boss.Direction = EnemyManager.DirectionType.Right;
                }
                else
                {
                    boss.Direction = EnemyManager.DirectionType.Left;
                }
            }
        }
    }

    /// <summary>
    /// このステートに入った際は左向きに移動し始める
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Enter(AIBrainBase brain, GameObject targetObj)
    {
        Debug.Log(_tagStr);
        EnemyManager boss = targetObj.GetComponent<EnemyManager>();
        if (boss != null)
        {
            boss.Direction = EnemyManager.DirectionType.Left;
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
    /// プレイヤーとの距離が一定値より近い時にAIActionApproachに変更
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public override (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        var symbol = brain.GetSymbolList();
        bool ret = false;
        string nextStateStr = null;

        if (symbol[AIBrainBase.StateSymbol.FoundPlayer])
        {
            nextStateStr = "approach";
            ret = true;
        }


        return (ret, nextStateStr);
    }
}
