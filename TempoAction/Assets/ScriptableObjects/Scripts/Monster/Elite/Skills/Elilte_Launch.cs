using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Launch", menuName = "ScriptableObjects/EliteMonster/Skill/Launch", order = 1)]
public class Elilte_Launch : Elite_Skill
{
    private float _coolTime;
    private float _totalTime;

    [SerializeField] private float _energyBallSpeed;
    private GameObject _energyBall;

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
            if (Vector2.Distance(_monster.Player.position, _monster.transform.position) <= _info.range) // �Ÿ� Ȯ��
            {              
                return true;
            }
        }
        else
        {
            _coolTime += Time.deltaTime;
        }

        return false;
    }

    public override void Enter()
    {
        Debug.Log("����ü");
        _monster.Direction = _monster.Player.transform.position.x - _monster.transform.position.x; // �÷��̾� �ٶ󺸱�

        _energyBall = ObjectPool.Instance.Spawn("ElectricBall");
        _energyBall.transform.position = _monster.StartEnergyBallPoint.position;

        _energyBall.GetComponent<ElectricBall>().TotalDamage = _monster.Stat.Damage * (_info.damage / 100);

    }
    public override void Stay()
    {
        _energyBall.transform.Translate(new Vector2(_monster.Direction, 0) * _energyBallSpeed * Time.deltaTime);

        if (_totalTime >= _info.totalTime)
        {
            _monster.FinishSkill();
        }
        else
        {
            _totalTime += Time.deltaTime;
        }

    }
    public override void Exit()
    {
        
        if (_energyBall.activeSelf)
        {
            GameObject explosion = ObjectPool.Instance.Spawn("ElectricBallExplosion");
            explosion.transform.position = _energyBall.transform.position;
            ObjectPool.Instance.Remove(_energyBall);
        }
        
        _totalTime = 0;
        _coolTime = 0;
    }

  
}
