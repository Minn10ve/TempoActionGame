using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barrier", menuName = "ScriptableObjects/EliteMonster/Skill/Barrier", order = 1)]
public class Elite_Barrier : Elite_Skill
{
    private float _coolTime;
    private float _totalTime;

    [SerializeField] private float _defense;   // ������ ���ҷ�

    private float _lastHp;

    public override void Init(EliteMonster monster)
    {
        base.Init(monster);

        _coolTime = 0;
        _totalTime = 0;
    }

    public override bool Check()
    {
        if (_coolTime >= _info.coolTime) // ��Ÿ�� Ȯ��
        {          
            return true;
        }
        else
        {
            _coolTime += Time.deltaTime;
        }

        return false;
    }

    public override void Enter()
    {
        Debug.Log("����");
        _lastHp = _monster.Stat.Health;
        _monster.Stat.Defense = _defense;
    }
    public override void Stay()
    {
        if (_totalTime >= _info.totalTime)
        {
            Debug.Log(_lastHp +" / "+ _monster.Stat.Health);
            if (_lastHp > _monster.Stat.Health) // �÷��̾ ���� ������ ���� ���� ��(ü�� ��ȭ�� ���� ��)
            {
                _monster.ChangeCurrentSkill(Define.EliteMonsterSkill.SUPERPUNCH); // �ָ� ġ�� ����
            }
            else
            {
                _monster.FinishSkill();
            }
            Debug.Log("���� ��");
        }
        else
        {
            _totalTime += Time.deltaTime;
        }
    }
    public override void Exit()
    {
        _monster.Stat.Defense = 0;
        _totalTime = 0;
        _coolTime = 0;
    }

}
