using System.Collections.Generic;
using UnityEngine;

/* ステートマシンでのAI */
public abstract class AIBrainBase
{
    
    /* シンボル情報 */
    public enum StateSymbol
    {
        FoundPlayer,  /* プレイヤーが認識できる距離にいるかどうか */
        Dead,         /* 死んでいるかどうか */
        FatalDamaged, /* 致命傷を受けているかどうか */
        ItemUsed,     /* アイテムをすでに使ったかどうか */
        Attackable    /* プレイヤーが攻撃範囲内にいるかどうか */
    };



    protected Dictionary<StateSymbol, bool> _symbolList;
    protected AISensor _sensor;

    public void SetSymbol(StateSymbol symbol, bool symbolVal)
    {
        if (_symbolList is null) return;

        if (_symbolList.ContainsKey(symbol))
        {
            _symbolList[symbol] = symbolVal;
            return;
        }

        _symbolList.Add(symbol, symbolVal);
    }

    public virtual void UpdateSensor(GameObject targetObj, Vector3 position)
    {
        
    }


    /// <summary>
    /// _sensorの機能を基に_symbolList更新
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="position"></param>
    public virtual void UpdateSymbol(GameObject targetObj, Vector3 position)
    {
        
    }

    /// <summary>
    /// _symbolListの状態を基に行動を選択
    /// </summary>
    /// <param name="targetObj"></param>
    public virtual void UpdateRoute(GameObject targetObj)
    {
        
    }

    /// <summary>
    /// 選択した行動を実施
    /// </summary>
    /// <param name="targetObj"></param>
    public virtual void ExecuteAction(GameObject targetObj)
    {
        
    }

    /// <summary>
    /// オブジェクトへ接触した際に呼ばれるコールバック
    /// </summary>
    /// <param name="targetObj"></param>
    public virtual void NotifyCollision(GameObject targetObj)
    {

    }


    public AISensor GetAISensor()
    {
        return _sensor;
    }

    public Dictionary<StateSymbol, bool> GetSymbolList()
    {
        return _symbolList;
    }
}
