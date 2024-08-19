using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkFinishState : IAtkState
{

    private Player _player;

    public AtkFinishState(Player player)
    {
        _player = player;
        
    }

    public void Enter()
    {
        _player.Ani.SetBool("FinishState", true);

        _player.Atk.HitMonsterList.Clear(); // ���� ���� �� ����Ʈ ����
        _player.Atk.AttackIndex = 0; // ���� �ε��� �ʱ�ȭ
      
    }

    public void Stay()
    {

    }

    public void Exit()
    {
        _player.Ani.SetBool("FinishState", false);
    }

}
