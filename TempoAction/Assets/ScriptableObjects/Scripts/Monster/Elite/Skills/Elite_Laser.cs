using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser", menuName = "ScriptableObjects/EliteMonster/Skill/Laser", order = 1)]
public class Elite_Laser : Elite_Skill
{
    private float _coolTime;
    private float _totalTime;

    [SerializeField] private float _laserLength = 50.0f; // �������� �ִ� ����
    [SerializeField] private float _laserWidth; // �������� ���� ����
    //private float _laserAngle; // �������� ���� (0���� ������)
    private GameObject _laser;
    public float LaserLength { get => _laserLength; }

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
       

        Debug.Log("������");

        Vector2 direction = _monster.Player.position - _monster.transform.position;
        _monster.Direction = direction.x;
        //_laserAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        CoroutineRunner.Instance.StartCoroutine(StartLaser());
    }
    public override void Stay()
    {
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
        _totalTime = 0;
        _coolTime = 0;
    }

    private IEnumerator StartLaser()
    {
        _monster.Ani.SetBool("Laser", true);

        yield return new WaitForSeconds(0.35f);

        _laser = ObjectPool.Instance.Spawn("Laser");
        _laser.transform.position = _monster.StartLaserPoint.position;

        Vector3 tempVec = _laser.transform.localScale;
        tempVec.x *= _monster.Direction;
        _laser.transform.localScale = tempVec;

        //_laser.transform.rotation = Quaternion.Euler(0, 0, _laserAngle);

        _laser.GetComponent<Laser>().TotalDamage = _monster.Stat.Damage * (_info.damage / 100);

        yield return new WaitForSeconds(1f);
        _laser.GetComponent<Collider>().enabled = true;

        yield return new WaitForSeconds(2f);

        _laser.GetComponent<Collider>().enabled = false;
        ObjectPool.Instance.Remove(_laser);
        _monster.Ani.SetBool("Laser", false);
    }

    /* // ������ �߻� �Լ�
     private void ShootLaser()
     {
         RaycastHit hit;

         // �������� ������ ������ ���� ���� (�⺻ ������ ������)
         Vector3 laserDirection = Quaternion.Euler(0, 0, _laserAngle) * Vector3.right;

         Vector3 point1 = _monster.StartLaserPoint.position;
         Vector3 point2 = Vector3.zero;

         if (Physics.Raycast(_monster.StartLaserPoint.position, laserDirection, out hit, _laserLength))
         {
             // �浹�� �߻��� ���, �������� ������ �浹 �������� ����
             _lineRenderer.SetPosition(0, point1);
             _lineRenderer.SetPosition(1, hit.point);

             point2 = hit.point;
         }
         else
         {
             // �浹�� �߻����� ���� ���, �������� ������ �ִ� �Ÿ��� ����
             _lineRenderer.SetPosition(0, point1);
             _lineRenderer.SetPosition(1, point1 + laserDirection * _laserLength);

             point2 = point1 + laserDirection * _laserLength;
         }

         Collider[] colliders = Physics.OverlapCapsule(point1, point2, _laserWidth / 2, _monster.PlayerLayer);

         foreach (Collider col in colliders)
         {
             Debug.Log("������ �浹");
             float damage = _monster.Stat.Damage * (_info.damage / 100);
             _monster.Player.GetComponent<Player>().TakeDamage(damage);
         }
     }*/
}
