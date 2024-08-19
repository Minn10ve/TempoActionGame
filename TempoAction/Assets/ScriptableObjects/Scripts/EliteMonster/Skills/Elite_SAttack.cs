using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SAttack", menuName = "ScriptableObjects/EliteMonster/Skill/SAttack", order = 1)]
public class Elite_SAttack : Elite_Skill
{
    private float _totalTime;

    private float _lastHealthPoints;                   // ������ ü�� ����
    [SerializeField] private float _reductionHealthPoints; // ü�� ���ҷ�

    private TempoCircle _tempoCircle;
    [SerializeField] private float _parringTime;

    public override void Init(EliteMonster monster)
    {
        base.Init(monster);

        _totalTime = 0;
        _lastHealthPoints = _monster.Stat.MaxHealthPoints;
    }

    public override void Check()
    {
        if (_isCompleted) return;

        if (_monster.Stat.HealthPoints + _reductionHealthPoints <= _lastHealthPoints) // ü�� ���� Ȯ��
        {
            if (Vector2.Distance(_monster.Player.position, _monster.transform.position) <= _info.range) // �Ÿ� Ȯ��
            {
                _isCompleted = true;
                _lastHealthPoints -= _reductionHealthPoints;
            }
        }
    }


    public override void Enter()
    {
        Debug.Log("�Ϲ� ����2");
        _monster.Player.GetComponent<Player>().Atk.CreateTempoCircle(_parringTime); // ����Ʈ ���� ����
        _tempoCircle = _monster.Player.GetComponent<Player>().Atk.PointTempoCircle;
    }
    public override void Stay()
    {
        if (_totalTime >= _info.totalTime)
        {
            Attack();
        }
        else
        {
            _totalTime += Time.deltaTime;
        }
    }
    public override void Exit()
    {
        _totalTime = 0;

        _monster.ResetSkill();
        _isCompleted = false;
    }

    public void Attack()
    {
        if (_tempoCircle.CircleState == Define.CircleState.GOOD || _tempoCircle.CircleState == Define.CircleState.PERFECT) // �и� ���� Ȯ��
        {
            Debug.Log("�и�");
            _monster.CurrentSkill = null;
            return;
        }

        bool isHit = Physics.CheckBox(_monster.HitPoint.position, _monster.ColliderSize / 2, _monster.HitPoint.rotation, _monster.PlayerLayer);

        if (isHit)
        {
            Debug.Log("�Ϲ� ����2 ����");
            float damage = _monster.Stat.AttackDamage * (_info.damage / 100);
            _monster.Player.GetComponent<Player>().Stat.TakeDamage(damage);
        }

        _monster.CurrentSkill = null;
    }
}
