using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "Rush", menuName = "ScriptableObjects/EliteMonster/Skill/Rush", order = 1)]
public class Elite_Rush : Elite_Skill
{
    private float _coolTime;

    private Tweener _rushTween;

    [SerializeField] private float _rushDistance;
    [SerializeField] private float _rushDuration;
    [SerializeField] AnimationCurve customCurve; // ����� ���� �ִϸ��̼� Ŀ��

    public override void Init(EliteMonster monster)
    {
        base.Init(monster);

        _coolTime = 0;
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

        _monster.Direction = _monster.Player.position.x - _monster.transform.position.x; // �÷��̾� �ٶ󺸱�

        // ����
        _rushTween = _monster.transform.DOMoveX(_monster.transform.position.x + _rushDistance * _monster.Direction, _rushDuration).SetEase(customCurve).OnUpdate(() =>
        {
            if (CheckHit()) // �÷��̾�� �浹 ��
            {
                _monster.FinishSkill();
                _rushTween.Kill();
            }

        }).OnComplete(() => { _monster.FinishSkill();  }); // �̵��� ���� ��

    }
    public override void Stay()
    {
    }

    public override void Exit()
    {
        _coolTime = 0;
    }

    // �浹 Ȯ�� �Լ�
    private bool CheckHit()
    {
        if (Physics.CheckBox(_monster.transform.position, _monster.RushColliderSize / 2, _monster.HitPoint.rotation, _monster.PlayerLayer))
        {
            float damage = _monster.Stat.Damage * (_info.damage / 100);
            _monster.Player.GetComponent<Player>().TakeDamage(damage);

            return true;
        }
        else if (Physics.CheckBox(_monster.transform.position, _monster.RushColliderSize / 2, _monster.HitPoint.rotation, _monster.WallLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
