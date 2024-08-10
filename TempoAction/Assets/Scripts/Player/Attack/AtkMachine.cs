using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class AtkMachine : MonoBehaviour
{
    private PlayerManager _player;

    [SerializeField] private List<AtkTempoData> _atkTempoDatas = new List<AtkTempoData>();

    [SerializeField] private int _index = 0;
    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
            _index = _index % 5;
            _player.Ani.SetInteger("AtkCount", _player.Atk.Index);

        }
    }


    public AtkTempoData CurAtkTempoData
    {
        get
        {
            if (_index == 4 && _player.Atk.UpgradeCount == 3) // ����Ʈ ���� ��ȭ Ȯ��
            {
                return _atkTempoDatas[5];
            }
            else
            {
                return _atkTempoDatas[_index];
            }
        }

    }

    public bool IsUpgraded { get; set; } = false;

    private int _upgradeCount = 0;
    public int UpgradeCount
    {
        get
        {
            return _upgradeCount;
        }
        set
        {
            _upgradeCount = value;
            _upgradeCount = _upgradeCount % 4;

            UIManager.Instance.GetUI<Slider>("UpgradeCountSlider").value = _upgradeCount;


        }
    }

    [SerializeField] private TempoCircle _pointTempoCircle;
    public TempoCircle PointTempoCircle { get { return _pointTempoCircle; } }
    public Define.CircleState CircleState { get; set; } = Define.CircleState.GOOD;

    private void Start()
    {


        _player = transform.parent.GetComponent<PlayerManager>();

    }

    private void Update()
    {
        if (_player.Stat.CheckOverload()) // ����ȭ üũ
        {
            //Debug.Log("����ȭ");
            if (_player.CurState == Define.PlayerState.NONE)
            {
                _player.CurState = Define.PlayerState.OVERLOAD;
            }
        }



    }

    public bool InputAtk()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            return true;
        }

        return false;
    }

    public void Execute() // ���� ����
    {
        if (_player.Ani.GetBool("isGrounded"))
        {
            _player.CurAtkState = Define.AtkState.ATTACK;
        }
    }


    public void StartPointTempCircle() // ����Ʈ ���� �� ����
    {
        if (CurAtkTempoData.type != Define.TempoType.POINT)
        {
            return;
        }
        //SoundManager.Instance.PlaySFX("SFX_PointTempo_Ready");
        _pointTempoCircle.gameObject.SetActive(true);
        _pointTempoCircle.ResetCircle();
    }

    #region �ִϸ��̼� �̺�Ʈ �Լ�

    public List<Enemy> HitEnemyList { get; set; } = new List<Enemy>();

    [Header(("�浹"))]
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Transform _hitPoint;

    [SerializeField] private Vector3 _colSize;
    [SerializeField] private LayerMask _enemyLayer;
    public Transform HitPoint { get { return _hitPoint; } }

    private bool _isHit = false;
    public float CheckDelay { get; set; } = 0;
    private void Hit()
    {
        //Collider[] hitEnemies = Physics.OverlapSphere(_hitPoint.position, _attackRadius, _enemyLayer);
        Collider[] hitEnemies = Physics.OverlapBox(_hitPoint.position, _colSize, _hitPoint.rotation, _enemyLayer);

        if (hitEnemies.Length <= 0)
        {
            return;
        }

        //SoundManager.Instance.PlaySFX("SFX_RhythmCombo_Hit1");

        _isHit = true;

        float damage = _player.Stat.Damage + CurAtkTempoData.damage;

        foreach (Collider enemy in hitEnemies)
        {

            // ������ ������
            enemy.GetComponent<Enemy>().Stat.TakeDamage(damage);

            // ��Ʈ ��ƼŬ ����
            GameObject hitParticle = null;
            if (CurAtkTempoData.type == Define.TempoType.POINT)
            {
                hitParticle = EffectManager.Instance.Pool.Spawn("P_point_attack", 1);
            }
            else
            {
                hitParticle = EffectManager.Instance.Pool.Spawn("P_main_attack", 1);
            }

            Vector3 hitPos = enemy.ClosestPoint(_hitPoint.position);
            hitParticle.transform.position = new Vector3(hitPos.x, hitPos.y, -0.1f);

            KnockBack(enemy.transform);
            HitEnemyList.Add(enemy.GetComponent<Enemy>());
        }

    }

    private void KnockBack(Transform target) // �˹�
    {
        Vector2 distance = target.position - _hitPoint.position;

        Vector2 endPoint = (Vector2)_endPoint.position + distance;

        target.DOMove(endPoint, 0.2f);
    }

    private void GravityActive(int value) // �߷� ����
    {
        foreach (Enemy enemy in HitEnemyList)
        {
            enemy.GetComponent<Rigidbody>().useGravity = (value == 0) ? false : true;
        }
    }

    private void Finish(float delay) // ������ ���� ���� 
    {
        if (_isHit)
        {
            float addStamina = CurAtkTempoData.maxStamina;

            if (CurAtkTempoData.type == Define.TempoType.POINT) // ����Ʈ ������ ��
            {
                if (CircleState == Define.CircleState.GOOD) // Ÿ�̹��� Good�� ���
                {
                    addStamina = CurAtkTempoData.minStamina;
                }
                
                UpgradeCount++;
            }

            _player.Stat.Stamina += addStamina;
            _isHit = false;
        }

        CheckDelay = delay;
        _player.CurAtkState = Define.AtkState.CHECK;

    }

    private void MoveToClosestEnemy(float duration) // Ư�� �Ÿ��� ���� ������ �� ������ �̵�
    {
        Vector3 rayOrigin = new Vector3(transform.parent.position.x, transform.parent.position.y+0.25f, transform.parent.position.z);
        Vector3 rayDirection = transform.localScale.x > 0 ? transform.right : transform.right * -1;

        // ����ĳ��Ʈ ��Ʈ ���� ����
        RaycastHit hit;

        // ����ĳ��Ʈ ����
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, CurAtkTempoData.distance, _enemyLayer))
        {
            Transform closestEnemy = hit.collider.GetComponent<Enemy>().moveToPoint;
            transform.parent.DOMoveX(closestEnemy.position.x, duration);
        }
 

        // ����׿� ���� �׸���
        Debug.DrawRay(rayOrigin, rayDirection * CurAtkTempoData.distance, Color.red);
    }


    private void ChangeTimeScale(float value) // �ð� ũ�� ����
    {
        Time.timeScale = value;
    }
    private void ReturnTimeScale() // �ð� ũ�� ����
    {
        Time.timeScale = 1;
    }

    private void StartTimeline(string name) //Ÿ�Ӷ��� ���� �Լ�
    {
        TimelineManager.Instance.PlayTimeline(name);
    }

  


    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colSize);
    }
 
}
