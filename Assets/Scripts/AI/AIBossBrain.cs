using System.Collections.Generic;
using UnityEngine;

public class AIBossBrain : AIBrainBase
{
    private const float JudgeHpRatio = 0.4f;
    private const float JudgeEnemyFoundDistance = 50f;
    private const float JudgeEnemyAttackableDistance = 1f;

    /* ステートリスト */
    private readonly AIActionApproach _stateApproach;
    private readonly AIActionAttack _stateAttack;
    private readonly AIActionWalkAround _stateWalkAround;
    private readonly AIActionGenerateMinion _stateGenerateMinion;
    private readonly AIActionGetItem _stateGetItem;

    private AIActionState _currentState;
    private AIActionState _nextState;

    public AIBossBrain()
    {
        /* 初期状態はすべてfalse */
        _symbolList = new Dictionary<StateSymbol, bool>
        {
            { StateSymbol.FoundPlayer   , false },
            { StateSymbol.Dead           , false },
            { StateSymbol.FatalDamaged  , false },
            { StateSymbol.ItemUsed      , false },
            { StateSymbol.Attackable     , false },
        };

        /* ステートリスト */
        _stateApproach = new AIActionApproach();
        _stateAttack = new AIActionAttack();
        _stateWalkAround = new AIActionWalkAround();
        _stateGenerateMinion = new AIActionGenerateMinion();
        _stateGetItem = new AIActionGetItem();

        /* センサー */
        _sensor = new AISensor();

        _currentState = _stateWalkAround;
        _nextState = null;
    }


    public override void UpdateSensor(GameObject targetObj, Vector3 position)
    {
        /* センサー値更新 */
        _sensor.UpdateNeighborObj(position, AISensor.GameObjectTag.Player);
        BossManager boss = targetObj.GetComponent<BossManager>();
        if (boss != null)
        {
            _sensor.CalcHpRatio(BossManager.MaxHp, boss.CurrentHp);
        }
    }

    /// <summary>
    /// _symbolList更新
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="position"></param>
    public override void UpdateSymbol(GameObject targetObj, Vector3 position)
    {
        /* 各シンボル更新 */
        if (JudgeEnemyFoundDistance > _sensor.DistanceToEnemy)
        {
            _symbolList[StateSymbol.FoundPlayer] = true;
        }
        else
        {
            _symbolList[StateSymbol.FoundPlayer] = false;
        }

        BossManager boss = targetObj.GetComponent<BossManager>();
        if (boss != null)
        {
            if (boss.CurrentHp <= 0f)
            {
                _symbolList[StateSymbol.Dead] = true;
            }
            else
            {
                _symbolList[StateSymbol.Dead] = false;
            }
        }

        if (JudgeHpRatio > _sensor.RatioHp)
        {
            _symbolList[StateSymbol.FatalDamaged] = true;
        }
        else
        {
            _symbolList[StateSymbol.FatalDamaged] = false;
        }

        if (JudgeEnemyAttackableDistance > Mathf.Abs(_sensor.NeighborEnemyPosition.x - position.x))
        {
            _symbolList[StateSymbol.Attackable] = true;
        }
        else
        {
            _symbolList[StateSymbol.Attackable] = false;
        }
    }

    /// <summary>
    /// _symbolListを参照して、適切なステートを見つける
    /// </summary>
    /// <param name="targetObj"></param>
    public override void UpdateRoute(GameObject targetObj)
    {
        /* 次のステートへの遷移条件を満たす場合は遷移 */
        if (ShouldChangeStateToGetItem()) /* グローバルステート。任意のステートから遷移する */
        {
            _nextState = _stateGetItem;
            Debug.Log("next: get item");
        }
        else
        {
            var (isChangedState, nextStr) = _currentState.ChangeState(this);
            if (isChangedState) /* 各ステート内での遷移条件を確認し、遷移 */
            {
                SetNextState(nextStr);
            }
        }
    }

    public override void NotifyCollision(GameObject targetObj)
    {
        _currentState.NotifyCollision(this, targetObj);
    }

    public bool IsFoundPlayer()
    {
        return (_currentState != _stateWalkAround); /* walkAroundState以外のステートに入っているとき、プレイヤーを認知していると考える */
    }

    /// <summary>
    /// _stateGetItemへ遷移する条件に入ったかどうかを判断。_stateGetItemは任意のステートから遷移する
    /// </summary>
    /// <returns></returns>
    private bool ShouldChangeStateToGetItem()
    {
        return _symbolList[StateSymbol.FatalDamaged] &&
               _symbolList[StateSymbol.ItemUsed] != true &&
               _currentState != _stateGetItem &&
               _nextState != _stateGetItem;
    }

    private void SetNextState(string tag)
    {
        _nextState = tag switch
        {
            "walkAround" => _stateWalkAround,
            "approach" => _stateApproach,
            "attack" => _stateAttack,
            "generateMinion" => _stateGenerateMinion,
            _ => null
        };
    }

    /// <summary>
    /// 選択されたステートへ変更し行動
    /// </summary>
    /// <param name="targetObj"></param>
    public override void ExecuteAction(GameObject targetObj)
    {
        if(_nextState == null)
        {
            _currentState.Execute(this, targetObj);
        }
        else
        {
            _currentState.Exit(this, targetObj);
            _nextState.Enter(this, targetObj);
            _currentState = _nextState;
            _nextState = null;
        }
    }

}
