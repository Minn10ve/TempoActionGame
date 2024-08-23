using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishState : PlayerAttackState
{
    public FinishState(Player player) : base(player)
    {

    }

    public override void Initialize()
    {

    }

    public override void Enter()
    {
        _player.Ani.SetBool("FinishState", true);

        _player.Attack.HitMonsterList.Clear(); // ���� ���� �� ����Ʈ ����
        _player.Attack.AttackIndex = 0; // ���� �ε��� �ʱ�ȭ

    }

    public override void Stay()
    {

    }

    public override void Exit()
    {
        _player.Ani.SetBool("FinishState", false);
    }
}
