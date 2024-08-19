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
    public Define.EliteMonsterState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != Define.EliteMonsterState.NONE)
            {
                _stateStroage[_currentState]?.Exit();
            }
            _currentState = value;
            _stateStroage[_currentState]?.Enter();
        }
    }
    public List<Elite_Skill> SkillStorage { get => _skillStorage; }
    public Elite_Skill CurrentSkill
    {
        get => _currentSkill;
        set
        {
            //print("Exit :" +_currentSkill);
            _currentSkill?.Exit();
            _currentSkill = value;

            //print("Enter :" + _currentSkill);
            _currentSkill?.Enter();
        }
    }
    public List<Elite_Skill> ReadySkills { get => _readySkills; set => _readySkills = value; }
    public float IdleDuration { get => _idleDuration; }
    public Transform HitPoint { get => _hitPoint; }
    public Vector3 ColliderSize { get => _colliderSize; }
    public Transform StartEnergyBallPoint { get => _startEnergyBallPoint; }
    public Transform StartLaserPoint { get => _startLaserPoint; }
    public Vector3 RushColliderSize { get => _rushColliderSize; }
    public CreatePlatform CreatePlatform { get => _createPlatform; }
    #endregion

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _rb = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>().transform;

        _stateStroage.Add(Define.EliteMonsterState.IDLE,new Elite_Idle(this));
        _stateStroage.Add(Define.EliteMonsterState.USESKILL, new Elite_UseSkill(this));
        _stateStroage.Add(Define.EliteMonsterState.GROGGY, new Elite_Groggy(this));
    }

    private void Start()
    {
       
        foreach (Elite_Skill s in _skillStorage)
        {
            s.Init(this);
        }

        CurrentState = Define.EliteMonsterState.IDLE;

    }

    private void Update()
    {
        if (_currentState != Define.EliteMonsterState.NONE)
        {
            _stateStroage[_currentState]?.Stay();
        }      
    }

    public void ResetSkill()
    {
        CurrentState = Define.EliteMonsterState.IDLE;
        _skillStorage.Add(_currentSkill);
    }


    public void ChangeSkill(Define.EliteMonsterSkill skill) // ��ų ��ü �Լ�
    {
        _skillStorage.Add(CurrentSkill);
        CurrentSkill = GetSkill(skill);      
    }

    public Elite_Skill GetSkill(Define.EliteMonsterSkill skill) // ��ų ã�� �Լ�
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colliderSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _rushColliderSize);
    }
}
