using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Phase2 : Elite_PhaseState
{
    public Elite_Phase2(ElitePhaseManager manager) : base(manager)
    {

    }

    public override void Enter()
    {
        _manager.Phase2Monster.gameObject.SetActive(true);
        _manager.Phase2Monster.Enter();
    }
    public override void Stay()
    {
        _manager.Phase2Monster.Stay();

        if (_manager.Phase2Monster.Stat.Health > 0)
        {
            if (_manager.Phase2Monster.Stat.Health <= _manager.TargetHealthList[_manager.TargetHealthIndex])
            {
                if (_manager.Phase2Monster.CurrentState == Define.EliteMonsterState.IDLE)
                {
                    _manager.Phase2Monster.ReadySkill(Define.EliteMonsterSkill.THUNDERSTROKE);
                    _manager.TargetHealthIndex++;
                }

            }
        }
        else
        {
            _manager.ChangeStageState(Define.ElitePhaseState.FINISH);
        }     
    }

    public override void Exit()
    {
        _manager.Phase2Monster.gameObject.SetActive(false);
    }

   
}
