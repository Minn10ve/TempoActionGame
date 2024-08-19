using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class AtkMachine : MonoBehaviour
{
    #region ����
    private Player _player;

    [SerializeField] private List<AtkTempoData> _atkTempoDatas = new List<AtkTempoData>();
    [SerializeField] private int _attackIndex = 0;

    private int _upgradeCount = 0;
    private TempoCircle _pointTempoCircle = null;
    #endregion

    #region ������Ƽ
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
    public AtkTempoData CurAtkTempoData { get=> _atkTempoDatas[_attackIndex]; }// ���� ���� ������ 
    public int UpgradeCount
    {
        get => _upgradeCount;
        set
        {
            _upgradeCount = value;
            _upgradeCount = _upgradeCount % 4;

            UIManager.Instance.GetUI<Slider>("UpgradeCountSlider").value = _upgradeCount;
        }
    }  
    public TempoCircle PointTempoCircle { get => _pointTempoCircle; set => _pointTempoCircle = value; }
    #endregion

    private void Start()
    {
        _player = transform.parent.GetComponent<Player>();
    }

    private void Update()
    {
        // ����ȭ üũ
        if (_player.Stat.CheckOverload()) 
        {
            if (_player.CurState == Define.PlayerState.NONE)
            {
                _player.CurState = Define.PlayerState.OVERLOAD;
            }
        }

        // ���� Ű �Է�
        if (_player.CurAtkState != Define.AtkState.ATTACK) 
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

    // ����Ʈ ���� ����
    private void SuccessTempoCircle() 
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

    // ����Ʈ ���� ����
    private void FailureTempoCircle()
    {
        _pointTempoCircle = null;
        _player.CurAtkState = Define.AtkState.FINISH;
    }

    // ���� �� ��
    private void FinishTempoCircle() 
    {
        
    }

    // ���� �� ����
    public void CreateTempoCircle(float duration = 1) 
    {
        SoundManager.Instance.PlayOneShot("event:/inGAME/SFX_PointTempo_Ready", transform);

        if (_pointTempoCircle == null)
        {
            GameObject tempoCircle = ObjectPool.Instance.Spawn("TempoCircle", 0, transform.parent);
            tempoCircle.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -0.1f);

            _pointTempoCircle = tempoCircle.GetComponent<TempoCircle>();
            _pointTempoCircle.Init(transform.parent);           // ���� �� �ʱ�ȭ

            _pointTempoCircle.ShrinkDuration = duration;        // ���� �� �ð� �� �߰�

            // ���� �̺�Ʈ �߰�
            _pointTempoCircle.OnSuccess += SuccessTempoCircle;
            _pointTempoCircle.OnFailure += FailureTempoCircle;
            _pointTempoCircle.OnFinish += FinishTempoCircle;
        }

    }
    #endregion


    #region �ִϸ��̼� �̺�Ʈ �Լ�

    private List<Monster> _hitMonsterList = new List<Monster>();
    public List<Monster> HitMonsterList { get => _hitMonsterList; set => _hitMonsterList = value; }

    [Header(("�浹"))]
    [SerializeField] private Transform _endPoint;    // �˹� ����
    [SerializeField] private Transform _hitPoint;    // �浹 ����

    [SerializeField] private Vector3 _colliderSize;
    [SerializeField] private LayerMask _monsterLayer;
   
    private bool _isHit = false;
    public float _checkDelay = 0; // üũ ���� ���� �ð�
   
    private float _knockBackTimer = 0;

    public Transform HitPoint { get { return _hitPoint; } }
    public float CheckDelay { get => _checkDelay; set => _checkDelay = value; }
   

    // �÷��̾� Ÿ�� ��ġ�� ����ϴ� �̺�Ʈ �Լ�
    private void Hit()
    {
        Collider[] hitMonsters = Physics.OverlapBox(_hitPoint.position, _colliderSize / 2, _hitPoint.rotation, _monsterLayer);

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


    // �˹� ��ġ�� ����ϴ� �̺�Ʈ �Լ�
    public void KnockBack(Monster monster) 
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

    // �˹� �� ��� ���� 
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

    // ����Ʈ ���� �ִϸ��̼� ���� �߰��ϴ� �̺�Ʈ �Լ�
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

    // ���� ���� �ִϸ��̼� ���� �߰��ϴ� �̺�Ʈ �Լ�
    private void Finish(float delay) 
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

    // ���� ��Ÿ� �ȿ� ���� ������ �� ������ �̵��ϴ� �̺�Ʈ �Լ�
    private void MoveToClosestMonster(float duration) 
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

    // �ð� ũ�� ����
    private void ChangeTimeScale(float value) 
    {
        Time.timeScale = value;
    }
    // �ð� ũ�� ����
    private void ReturnTimeScale() 
    {
        Time.timeScale = 1;
    }
    //Ÿ�Ӷ��� ���� �Լ�
    private void StartTimeline(string name)
    {
        TimelineManager.Instance.PlayTimeline(name);
    }

    // �߷� ����
    private void GravityActive(int value) 
    {
        foreach (Monster monster in HitMonsterList)
        {
            monster.Rb.useGravity = (value == 0) ? false : true;
        }
    }

    // �ݶ��̴� Ȱ��ȭ/��Ȱ��ȭ
    private void SetColliderActive(int value) 
    {
        _player.GetComponent<Collider>().enabled = (value == 0) ? false : true;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colliderSize);
    }
 
}
