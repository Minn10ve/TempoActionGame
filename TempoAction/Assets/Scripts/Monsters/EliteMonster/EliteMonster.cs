using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonster : Monster
{
    #region ����

    // ����
    private Dictionary<Define.EliteMonsterState, Elite_State> _stateStroage = new Dictionary<Define.EliteMonsterState, Elite_State>(); // ���� �����
    [SerializeField] private Define.EliteMonsterState _currentState = Define.EliteMonsterState.NONE;                                   // ���� ����

    // ��ų
    [SerializeField] private List<Elite_Skill> _skillStorage = new List<Elite_Skill>();  // ��ų �����
    [SerializeField] private Elite_Skill _currentSkill = null;                           // ���� ��ų
    [SerializeField] private List<Elite_Skill> _readySkills = new List<Elite_Skill>();   // �غ�� ��ų

    [SerializeField] private float _idleDuration;                                        // ��� ���� �ð�

    [Header("�Ϲ� ����1")]
    [SerializeField] private Transform _hitPoint;
    [SerializeField] private Vector3 _colliderSize;

    [Header("��������")]
    [SerializeField] private Transform _startEnergyBallPoint; // �������� ���� ����

    [Header("������")]
    [SerializeField] private Transform _startLaserPoint; // �������� ���� ����

    [Header("����")]
    [SerializeField] private Vector3 _rushColliderSize;

    [Header("����")]
    [SerializeField] private CreatePlatform _createPlatform;
    #endregion;

    #region ������Ƽ

    public List<Elite_Skill> SkillStorage { get => _skillStorage; }
    public Elite_Skill CurrentSkill { get => _currentSkill; }
    public List<Elite_Skill> ReadySkills { get => _readySkills; set => _readySkills = value; }
    public float IdleDuration { get => _idleDuration; }
    public Transform HitPoint { get => _hitPoint; }
    public Vector3 ColliderSize { get => _colliderSize; }
    public Transform StartEnergyBallPoint { get => _startEnergyBallPoint; }
    public Transform StartLaserPoint { get => _startLaserPoint; }
    public Vector3 RushColliderSize { get => _rushColliderSize; }
    public CreatePlatform CreatePlatform { get => _createPlatform; }
    #endregion

    protected override void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>().transform;

        _stateStroage.Add(Define.EliteMonsterState.IDLE, new Elite_Idle(this));
        _stateStroage.Add(Define.EliteMonsterState.USESKILL, new Elite_UseSkill(this));
        _stateStroage.Add(Define.EliteMonsterState.GROGGY, new Elite_Groggy(this));

        _stat.Initialize();
    }

    private void Start()
    {

        foreach (Elite_Skill s in _skillStorage)
        {
            s.Init(this);
        }

        _currentState = Define.EliteMonsterState.NONE;

    }

    private void Update()
    {
        if (_currentState != Define.EliteMonsterState.NONE)
        {
            _stateStroage[_currentState]?.Stay();
        }
    }

    public void ChangeCurrentState(Define.EliteMonsterState state)
    {
        if (_currentState != Define.EliteMonsterState.NONE)
        {
            _stateStroage[_currentState]?.Exit();
        }
        _currentState = state;
        _stateStroage[_currentState]?.Enter();
    }


    #region ��ų

    // ��ų�� ������ �� ����ϴ� �Լ�
    public void FinishSkill()
    {
        _skillStorage.Add(_currentSkill); // ���� ����ҷ� �̵�
        _currentSkill?.Exit();

        _currentSkill = null;

        ChangeCurrentState(Define.EliteMonsterState.IDLE);
    }

    // ���� ��ų ��ü �Լ�
    public void ChangeCurrentSkill(Define.EliteMonsterSkill skill)
    {
        if (_currentSkill != null)
        {
            _skillStorage.Add(_currentSkill); // ���� ����ҷ� �̵�     
            _currentSkill?.Exit();
        }

        if (skill == Define.EliteMonsterSkill.NONE)
        {
            _currentSkill = null;
        }
        else
        {
            _currentSkill = GetSkill(skill);
            _currentSkill?.Enter();
        }  
    }
    public void ChangeCurrentSkill(Elite_Skill skill)
    {
        if (_currentSkill != null)
        {
            _skillStorage.Add(_currentSkill); // ���� ����ҷ� �̵�     
            _currentSkill?.Exit();
        }
        _currentSkill = skill;
        _currentSkill?.Enter();
    }


    // ��ų ã�� �Լ�
    public Elite_Skill GetSkill(Define.EliteMonsterSkill skill) 
    {
        foreach (Elite_Skill s in _skillStorage)
        {
            if (s.Info.skill == skill)
            {
                _skillStorage.Remove(s);
                return s;
            }
        }

        return null;
    }

    #endregion

   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colliderSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _rushColliderSize);
    }
}
