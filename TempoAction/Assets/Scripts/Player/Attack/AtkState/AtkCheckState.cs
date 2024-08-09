using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkCheckState : IAtkState
{

    private PlayerManager _player;


    public AtkCheckState(PlayerManager player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.Ani.SetBool("CheckState", true);

        if (_player.Atk.CurAtkTempoData.type == Define.TempoType.POINT)
        {
            _player.Atk.StartPointTempCircle();
        }
    }

    public void Exit()
    {
        _player.Ani.SetBool("CheckState", false);
    }

    public void Stay()
    {

        if (_player.Atk.CurAtkTempoData.type == Define.TempoType.POINT)
        {
            if (_player.Atk.CircleState == Define.CircleState.GOOD || _player.Atk.CircleState == Define.CircleState.PERFECT)
            {
                _player.Atk.Execute();
            }
        }
        else
        {
            if (Input.GetKeyDown(InputManager.Instance.FindKeyCode("MainTempo")))
            {
                _player.Atk.Execute();
            }

        }


        if (_player.Atk.CheckDelay <= 0)
        {
            _player.CurAtkState = Define.AtkState.FINISH;
        }
        else
        {
            _player.Atk.CheckDelay -= Time.deltaTime;
        }

    }


}
