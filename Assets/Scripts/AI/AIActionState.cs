using UnityEngine;

public abstract class AIActionState
{
    // 前提条件
    protected string _tagStr;

    public AIActionState()
    {

    }

    /// <summary>
    /// このステートに変更された際に一度だけ呼ばれる
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public virtual void Enter(AIBrainBase brain, GameObject targetObj)
    {

    }

    /// <summary>
    /// ステートの実行関数。一フレームに一度呼ばれる
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public virtual void Execute(AIBrainBase brain, GameObject targetObj)
    {

    }

    /// <summary>
    /// ステートがこのステートから別のステートへ変更される時に一度呼ばれる
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="targetObj"></param>
    public virtual void Exit(AIBrainBase brain, GameObject targetObj)
    {

    }

    /// <summary>
    /// ステート変更をすべきかどうかを判断する。返り値がtrueであれば、引数に次のステートのタグ文字列を入れる
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public virtual (bool isChangedState, string nextStateTag) ChangeState(AIBrainBase brain)
    {
        return (false, null);
    }

    /// <summary>
    /// オブジェクトに接触した際に呼ばれるコールバック
    /// </summary>
    /// <param name="brain"></param>
    /// <param name="collisionObj"></param>
    public virtual void NotifyCollision(AIBrainBase brain, GameObject collisionObj)
    {

    }
}
