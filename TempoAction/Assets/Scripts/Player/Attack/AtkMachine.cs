using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class AtkMachine : MonoBehaviour
{
    private Player _player;

    [SerializeField] private List<AtkTempoData> _atkTempoDatas = new List<AtkTempoData>();

    [SerializeField] private int _attackIndex = 0;
    public int AttackIndex
    {
        get
        {
            return _attackIndex;
        }
        set
        {
            _attackIndex = value;

            if (_attackIndex == 4)
            {
                CreateTempoCircle();
            }

            _player.Ani.SetInteger("AtkCount", _attackIndex);
            
        }
    }


    public AtkTempoData CurAtkTempoData // ���� ���� ������ 
    {
        get
        {
            return _atkTempoDatas[_attackIndex];
        }

    }

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

    private TempoCircle _pointTempoCircle = null;
    public TempoCircle PointTempoCircle 
    { 
        get => _pointTempoCircle;
        set 
        {
            _pointTempoCircle = value; 
        }

    }


    private bool _isParrying;
    public bool IsParrying { get => _isParrying; set => _isParrying = value; }

    private void Start()
    {
        _player = transform.parent.GetComponent<Player>();

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

        if (_player.CurAtkState != Define.AtkState.ATTACK) // ���� Ű �Է�
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _player.Atk.AttackMainTempo();
            }
        }
    }

    #region ���� ����
    public void AttackMainTempo() // ���� ����
    {
        if (_pointTempoCircle != null) return;

        if (_player.Ani.GetBool("isGrounded"))
        {
            _player.CurAtkState = Define.AtkState.ATTACK;
        }
    }

    #endregion

    #region ����Ʈ ����

    private void SuccessTempoCircle() // ����Ʈ ���� ����
    {
        if (_upgradeCount == 3) // ����Ʈ ���� ���׷���Ʈ Ȯ��
        {
            AttackIndex = 5;
            _player.Ani.SetBool("IsUpgraded", true);
        }
        else
        {
            AttackIndex = 4;
            _player.Ani.SetBool("IsUpgraded", false);
        }

        _player.Ani.SetTrigger("PointTempo");

        _player.CurAtkState = Define.AtkState.ATTACK;
    }

    private void FailureTempoCircle()
    {
        _pointTempoCircle = null;
        _player.CurAtkState = Define.AtkState.FINISH;
    }

    private void FinishTempoCircle() // ���� �� ��
    {
        
    }

    public void CreateTempoCircle(float duration = 1) // ���� �� ����
    {
        //SoundManager.Instance.PlayOneShot("event:/inGAME/SFX_PointTempo_Ready", transform);

        if (_pointTempoCircle == null)
        {
            GameObject tempoCircle = ObjectPool.Instance.Spawn("TempoCircle", 0, transform.parent);
            tempoCircle.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -0.1f);

            _pointTempoCircle = tempoCircle.GetComponent<TempoCircle>();
            _pointTempoCircle.Init(transform.parent);

            _pointTempoCircle.ShrinkDuration = duration;

            _pointTempoCircle.OnSuccess += SuccessTempoCircle;
            _pointTempoCircle.OnFailure += FailureTempoCircle;
            _pointTempoCircle.OnFinish += FinishTempoCircle;
        }

    }


    #endregion




    #region �ִϸ��̼� �̺�Ʈ �Լ�

    public List<Monster> HitMonsterList { get; set; } = new List<Monster>();

    [Header(("�浹"))]
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Transform _hitPoint;

    [SerializeField] private Vector3 _colSize;
    [SerializeField] private LayerMask _monsterLayer;
    public Transform HitPoint { get { return _hitPoint; } }

    private bool _isHit = false;
    public float CheckDelay { get; set; } = 0;
    private void Hit()
    {
        Collider[] hitMonsters = Physics.OverlapBox(_hitPoint.position, _colSize/2, _hitPoint.rotation, _monsterLayer);

        if (hitMonsters.Length <= 0)
        {
            return;
        }

        if (_isHit == false && CurAtkTempoData.type == Define.TempoType.MAIN)
        {
            SoundManager.Instance.PlayOneShot("event:/inGAME/SFX_RhythmCombo_Hit_" + (_attackIndex + 1), transform);
        }
        _isHit = true;

        float damage = _player.Stat.AttackDamage + CurAtkTempoData.damage;

        foreach (Collider monster in hitMonsters)
        {

            Monster m = monster.GetComponent<Monster>();


            // ������ ������
            if (CurAtkTempoData.type == Define.TempoType.POINT)
            {
                m.Stat.TakeDamage(damage, true);
            }
            else
            {
                m.Stat.TakeDamage(damage);
            }

            if (m.CanKnockback)
            {
                KnockBack(m);
            }

            // ��Ʈ ��ƼŬ ����
            GameObject hitParticle = null;
            if (CurAtkTempoData.type == Define.TempoType.POINT)
            {
         
                hitParticle = ObjectPool.Instance.Spawn("P_point_attack", 1);
            }
            else
            {
          
                hitParticle = ObjectPool.Instance.Spawn("P_main_attack", 1);
            }

            Vector3 hitPos = monster.ClosestPoint(_hitPoint.position);
            hitParticle.transform.position = new Vector3(hitPos.x, hitPos.y, -0.1f);

            HitMonsterList.Add(monster.GetComponent<Monster>());
        }

    }

    private float _knockBackTimer = 0;

    public void KnockBack(Monster monster) // �˹�
    {
        Vector2 distance = monster.transform.position - _hitPoint.position;

        Vector2 ep = (Vector2)_endPoint.position + distance;

        _knockBackTimer = 0;
        monster.Stat.IsKnockBack = true;


        monster.transform.DOMove(ep, 0.2f).OnComplete(() =>
        {
            monster.Stat.IsKnockBack = false;

            if (!monster.Stat.IsStunned)
            {
                StartCoroutine(Stun(monster));

            }

        });


    }

    private IEnumerator Stun(Monster monster)
    {
        monster.Stat.IsStunned = true;
        while (_knockBackTimer < 0.5f)
        {
            _knockBackTimer += Time.deltaTime;

            yield return null;
        }
        monster.Stat.IsStunned = false;

        monster.OnKnockback?.Invoke();
    }

    private void FinishPointTempo()
    {
        float addStamina = 0;

        if (CurAtkTempoData.type == Define.TempoType.POINT) // ����Ʈ ������ ��
        {
            if (_isHit)
            {
                addStamina = CurAtkTempoData.maxStamina;

                if (_pointTempoCircle.CircleState == Define.CircleState.GOOD) // Ÿ�̹��� Good�� ���
                {
                    addStamina = CurAtkTempoData.minStamina;
                }

                UpgradeCount++;

            }

        }

        _player.Stat.Stamina += addStamina;
        _isHit = false;

        _pointTempoCircle = null;
        _player.CurAtkState = Define.AtkState.FINISH;
    }

    private void Finish(float delay) // ������ ���� ���� 
    {

        float addStamina = 0;

        if (_isHit)
        {
            addStamina = CurAtkTempoData.maxStamina;
        }


        _player.Stat.Stamina += addStamina;
        _isHit = false;

        CheckDelay = delay;
        _player.CurAtkState = Define.AtkState.CHECK;

      
        AttackIndex++;
        

    }

    private void MoveToClosestMonster(float duration) // Ư�� �Ÿ��� ���� ������ �� ������ �̵�
    {
        Vector3 rayOrigin = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z);
        Vector3 rayDirection = transform.localScale.x > 0 ? transform.right : transform.right * -1;

        // ����ĳ��Ʈ ��Ʈ ���� ����
        RaycastHit hit;

        // ����ĳ��Ʈ ����
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, CurAtkTempoData.distance, _monsterLayer))
        {
            float closestMonsterX = hit.transform.position.x + (-rayDirection.x * 0.75f);
            transform.parent.DOMoveX(closestMonsterX, duration);
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


    private void GravityActive(int value) // �߷� ����
    {
        foreach (Monster monster in HitMonsterList)
        {
            monster.Rb.useGravity = (value == 0) ? false : true;
        }
    }

    private void SetColliderActive(int value) // �ݶ��̴� Ȱ��ȭ/��Ȱ��ȭ
    {
        _player.GetComponent<Collider>().enabled = (value == 0) ? false : true;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colSize);
    }
 
}
