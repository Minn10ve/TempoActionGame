using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverloadState : IPlayerState
{
    private float timer = 0;
    private Player _player;

    public OverloadState(Player player)
    {
        _player = player;
    }

    public void Enter()
    {
        timer = 0;
    }

    public void Stay()
    {
        
        if (timer < _player.Stat.StunDelay)
        {
            if (_player.Atk.CurAtkTempoData.type != Define.TempoType.POINT)
            {
                
               // Debug.Log("����ȭ �ð� ������.....");
                timer += Time.deltaTime;
            }

            if (!_player.Stat.CheckOverload()) // ����ȭ ���°� �ƴ� ��
            {
                //Debug.Log("Ż��");
                _player.CurState = Define.PlayerState.NONE;
            }

        }
        else
        {
            _player.CurState = Define.PlayerState.STUN;
        }
    }

    public void Exit()
    {
       
    }
}
