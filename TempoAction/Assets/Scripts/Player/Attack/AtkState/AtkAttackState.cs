using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkAttackState : IAtkState
{
    private PlayerManager _player;

    public AtkAttackState(PlayerManager player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.Ani.SetTrigger("IsAttacking");
        _player.Ani.SetInteger("AtkCount", _player.Atk.Index);

        if (_player.Atk.UpgradeCount == 3) // 포인트 템포 강화 확인
        {
            _player.Ani.SetBool("IsUpgraded", true);
        }
        else
        {
            _player.Ani.SetBool("IsUpgraded", false);
        }
    }

    public void Exit()
    {
        _player.Atk.Index++;

    }

    public void Stay()
    {
  
    }

   
}
