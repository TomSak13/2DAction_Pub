using System.Collections.Generic;
using UnityEngine;

public class AIActionAttack : AIActionState
{
    private static readonly int _attackAnimTrigger = Animator.StringToHash("attack"); /* 速度改善のため、事前にハッシュ値を取得しておく */
    public AIActionAttack()
    {
        _tagStr = "attack";
    }

    /// <summary>
    /// プレイヤーへ攻撃
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Execute(AIBrainBase brain, GameObject targetObj)
    {
        BossManager boss = targetObj.GetComponent<BossManager>();
        if (boss != null)
        {
            boss.Direction = EnemyManager.DirectionType.Stop;
            boss.BossAnimator.SetTrigger(_attackAnimTrigger);
        }
    }

    /// <summary>
    /// このステートに入ったら一度進行方向をリセット。攻撃時に移動しないようにするため
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Enter(AIBrainBase brain, GameObject targetObj)
    {
        Debug.Log(_tagStr);
        BossManager boss = targetObj.GetComponent<BossManager>();
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
    /// プレイヤーが攻撃範囲外へ移動したらAIActionApproachに変更
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public override (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        var symbol = brain.GetSymbolList();
        bool ret = false;
        string nextStateStr = null;

        if (!symbol[AIBrainBase.StateSymbol.Attackable])
        {
            nextStateStr = "approach";
            ret = true;
        }

        return (ret, nextStateStr);
    }
}
