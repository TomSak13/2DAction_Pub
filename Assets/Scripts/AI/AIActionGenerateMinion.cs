using UnityEngine;

public class AIActionGenerateMinion : AIActionState
{
    private bool _isGenerated;
    public AIActionGenerateMinion()
    {
        _tagStr = "generateMinion";
        _isGenerated = false;
    }

    /// <summary>
    /// 速度を止め、攻撃アニメーションを実施。アニメーション後に雑魚敵が生成されるようにするため
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Enter(AIBrainBase brain, GameObject targetObj)
    {
        Debug.Log(_tagStr);
        /* 速度を止め。Attackアニメーションを行う */
        BossManager boss = targetObj.GetComponent<BossManager>();
        boss.Direction = EnemyManager.DirectionType.Stop;
    }

    /// <summary>
    /// 雑魚敵を生成。このステートに入る度に生成する。
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public override void Execute(AIBrainBase brain, GameObject targetObj)
    {
        BossManager boss = targetObj.GetComponent<BossManager>();
        if (boss != null) 
        {
            boss.BossAnimator.SetTrigger("attack");

            boss.GenerateMinion();

            _isGenerated = true;
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
    /// 雑魚敵を1体生成したら、AIActionApproachに変更
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public override (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        bool ret = false;
        string nextStateStr = null;
        /* 一体minionを出したらステートを変更する */
        if (_isGenerated)
        {
            nextStateStr = "approach";
            ret = true;
            _isGenerated = false;
        }

        return (ret, nextStateStr);
    }
}
