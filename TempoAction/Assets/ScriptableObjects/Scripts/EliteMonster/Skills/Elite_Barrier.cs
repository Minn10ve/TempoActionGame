using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barrier", menuName = "ScriptableObjects/EliteMonster/Skill/Barrier", order = 1)]
public class Elite_Barrier : Elite_Skill
{
    private float _coolTime;
    private float _totalTime;

    [SerializeField] private float _damageReduction;   // ������ ���ҷ�
    [SerializeField] private float _knockBackPower;    // �˹� �Ŀ�
    [SerializeField] private float _knockBackDuration; // �˹� �ð�

    private float _lastHp;

    public override void Init(EliteMonster monster)
    {
        base.Init(monster);

        _coolTime = 0;
        _totalTime = 0;
    }

    public override void Check()
    {
        if (_isCompleted) return;

        if (_coolTime >= _info.coolTime) // ��Ÿ�� Ȯ��
        {
            _coolTime = 0;
            _isCompleted = true;
        }
        else
        {
            _coolTime += Time.deltaTime;
        }
    }


    public override void Enter()
    {
        Debug.Log("����");
        _lastHp = _monster.Stat.HealthPoints;
        _monster.Stat.DamageReduction = _damageReduction;
    }
    public override void Stay()
    {
        if (_totalTime >= _info.totalTime)
        {
            if (_lastHp > _monster.Stat.HealthPoints) // �÷��̾ ���� ������ ���� ���� ��(ü�� ��ȭ�� ���� ��)
            {
                _monster.ChangeSkill(Define.EliteMonsterSkill.SUPERPUNCH); // �ָ� ġ�� ����
                _monster.CurrentSkill.IsCompleted = true;
            }
            else
            {
                _monster.CurrentSkill = null;
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
        _monster.Stat.DamageReduction = 0;
        _totalTime = 0;

        if (_lastHp == _monster.Stat.HealthPoints) // ü�� ��ȭ�� ���� ��
        {
            _monster.ResetSkill();
        }

        _isCompleted = false;
    }

 /*   public void Attack()
    {

        bool isHit = Physics.CheckBox(_monster.HitPoint.position, _monster.ColSize / 2, _monster.HitPoint.rotation, _monster.PlayerLayer);

        if (isHit)
        {
            Debug.Log("�ָ� ġ�� ����");
            float damage = _monster.Stat.AttackDamage * (_info.damage / 100);
            _monster.Player.GetComponent<Player>().Stat.TakeDamage(damage);
            _monster.Player.GetComponent<Player>().Stat.Knockback(Vector2.right * _monster.Direction * _knockBackPower, _knockBackDuration);

        }

    }*/
}
