using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Laser", menuName = "ScriptableObjects/EliteMonster/Skill/Laser", order = 1)]
public class Elite_Laser : Elite_Skill
{
    private LineRenderer _lineRenderer; // Line Renderer ������Ʈ

    [SerializeField] private float _laserLength = 50.0f; // �������� �ִ� ����
    public float LaserLength { get => _laserLength; }

    [SerializeField] private float _laserWidth; // �������� ���� ����
    private float _laserAngle; // �������� ���� (0���� ������)

    private float _coolTime;
    private float _totalTime;

    public override void Init(EliteMonster monster)
    {
        base.Init(monster);

        _lineRenderer = _monster.StartLaserPoint.GetComponent<LineRenderer>();
        _coolTime = 0;
        _totalTime = 0;
    }

    public override void Check()
    {
        if (_isCompleted) return;


        if (_coolTime >= _info.coolTime)
        {
            if (Vector2.Distance(_monster.Player.position, _monster.transform.position) <= _info.range)
            {

                _coolTime = 0;
                _isCompleted = true;
            }

        }
        else
        {
            _coolTime += Time.deltaTime;
        }
    }

    public override void Enter()
    {
        Debug.Log("������");
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;

        Vector2 direction = _monster.Player.position - _monster.transform.position;
        _monster.Direction = direction.x;
        _laserAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    }
    public override void Stay()
    {

        if (_totalTime >= _info.totalTime)
        {
            _monster.CurrentSkill = null;

        }
        else
        {
            _totalTime += Time.deltaTime;
            ShootLaser();
        }


    }
    public override void Exit()
    {
        _lineRenderer.positionCount = 0;
        _totalTime = 0;
        _monster.ResetSkill();
        _isCompleted = false;
    }

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
            float damage = _monster.Stat.AttackDamage * (_info.damage / 100);
            _monster.Player.GetComponent<Player>().Stat.TakeDamage(damage);
        }



    }
}
