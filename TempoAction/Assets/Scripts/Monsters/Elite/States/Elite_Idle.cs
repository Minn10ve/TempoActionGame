using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elite_Idle : Elite_State
{
    private float idleTime; // �ð� üũ��

    public Elite_Idle(EliteMonster monster) : base(monster)
    {

    }

    public override void Enter()
    {
        idleTime = 0;
    }
    public override void Stay()
    {

        Follow();
        if (idleTime < _monster.IdleDuration)
        {
            idleTime += Time.deltaTime;
        }
        else
        {

            foreach (Elite_Skill s in _monster.SkillStorage)
            {
                if (s.Check()) // ������ �����Ǿ����� Ȯ��
                {
                    _monster.ReadySkills.Add(s);
                }
            }

            if (_monster.ReadySkills.Count <= 0) return;

            Elite_Skill prioritySkill = _monster.ReadySkills[0];
            _monster.SkillStorage.Remove(prioritySkill);

            if (_monster.ReadySkills.Count > 1) // 2�� �̻��� �� �켱���� Ȯ��
            {
                for (int i = 1; i < _monster.ReadySkills.Count; i++)
                {
                    if (prioritySkill.Info.priority < _monster.ReadySkills[i].Info.priority)
                    {
                        prioritySkill = _monster.ReadySkills[i];
                    }

                    _monster.SkillStorage.Remove(_monster.ReadySkills[i]);
                }
            }
            _monster.ChangeCurrentState(Define.EliteMonsterState.USESKILL);
            _monster.ChangeCurrentSkill(prioritySkill);
            _monster.ReadySkills.Remove(prioritySkill);

        }

    }
    public override void Exit()
    {
        _monster.Rb.velocity = new Vector2(0, _monster.Rb.velocity.y);
    }

    // �÷��̾� ���� �Լ�
    private void Follow()
    {
        float direction = _monster.Player.transform.position.x - _monster.transform.position.x;
        _monster.Direction = direction;

        if (Mathf.Abs(direction) <= _monster.Stat.AttackRange)
        {
            _monster.Rb.velocity = new Vector2(0, _monster.Rb.velocity.y);
        }
        else
        {
            _monster.Rb.velocity = new Vector2(_monster.Direction * _monster.Stat.SprintSpeed, _monster.Rb.velocity.y);
        }
    }
}
