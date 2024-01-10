using System.Collections.Generic;
using UnityEngine;

public class AIMonsterDogBrain : AIBrainBase
{
    /// <summary>
    /// ステートリスト
    /// </summary>
    private readonly AIActionApproach _stateApproach;

    public AIMonsterDogBrain()
    {
        /* ステートリスト */
        _stateApproach = new AIActionApproach();

        /* センサー */
        _sensor = new AISensor();
    }

    /// <summary>
    /// プレイヤーの位置確認のため、_sensor更新
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="position"></param>
    public override void UpdateSensor(GameObject targetObj, Vector3 position)
    {
        /* センサー値更新 */
        _sensor.UpdateNeighborObj(position, AISensor.GameObjectTag.Player);
    }

    /// <summary>
    /// プレイヤーを追いかけるだけなので何もしない
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="position"></param>
    public override void UpdateSymbol(GameObject targetObj, Vector3 position)
    {
        
    }

    /// <summary>
    /// プレイヤーを追いかけるだけなので何もしない
    /// </summary>
    /// <param name="targetObj"></param>
    public override void UpdateRoute(GameObject targetObj)
    {
        
    }

    /// <summary>
    /// プレイヤーを追いかけるAIActionApproachを実行する
    /// </summary>
    /// <param name="targetObj"></param>
    public override void ExecuteAction(GameObject targetObj)
    {
        _stateApproach.Execute(this, targetObj);
    }
}
