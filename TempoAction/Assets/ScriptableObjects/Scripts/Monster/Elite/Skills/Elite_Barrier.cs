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

    private GameObject _guardEffect;

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

        // ���� ����Ʈ ����
        _guardEffect = ObjectPool.Instance.Spawn("FX_EliteGuard");
        _guardEffect.transform.position = _monster.transform.position;

        _monster.GetComponent<BoxCollider>().size = new Vector3(2, 1, 1);

        _monster.IsGuarded = true;
    }
    public override void Stay()
    {
        if (_totalTime >= _info.totalTime)
        {
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
        _monster.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);

        // ���� ����Ʈ ����
        ObjectPool.Instance.Remove(_guardEffect);
      

        _monster.Stat.Defense = 0;
        _totalTime = 0;
        _coolTime = 0;

        _monster.IsGuarded = false;
    }

}
