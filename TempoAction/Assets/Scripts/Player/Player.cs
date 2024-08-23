using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    private PlayerView _view;

    private PlayerAttack _attack;
    private PlayerController _controller;

    private Rigidbody _rb;
    private Animator _ani;

    [SerializeField] private Define.PlayerState _currentState = Define.PlayerState.NONE;
    private Dictionary<Define.PlayerState, PlayerState> _stateStorage = new Dictionary<Define.PlayerState, PlayerState>();

    [SerializeField] private Transform _playerModel;

    [SerializeField] private Transform _rightSparkPoint;
    [SerializeField] private Transform _leftSparkPoint;

    [Header("������")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    [Header("����")]
    [SerializeField] private Transform _hitPoint;
    [SerializeField] private Transform _endPoint;    // �˹� ����
    [SerializeField] private Vector3 _colliderSize;
    [SerializeField] private LayerMask _monsterLayer;

    [SerializeField] private List<TempoAttackData> _tempoAttackDatas;

    public PlayerStat Stat { get { return _stat; } }
    public PlayerAttack Attack { get { return _attack; } }
    public PlayerController Controller { get { return _controller; } }
    public Rigidbody Rb { get { return _rb; } }
    public Animator Ani { get { return _ani; } }
    public Define.PlayerState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _stateStorage[_currentState]?.Exit();
            _currentState = value;
            _stateStorage[_currentState]?.Enter();
        }
    }
   
    public Transform PlayerModel { get => _playerModel; }
    public Transform RightSparkPoint { get => _rightSparkPoint; }
    public Transform LeftSparkPoint { get => _leftSparkPoint; }
    public Transform GroundCheckPoint { get => _groundCheckPoint; }
    public float GroundCheckRadius { get => _groundCheckRadius; }
    public LayerMask GroundLayer { get => _groundLayer; }
    public LayerMask WallLayer { get => _wallLayer; }

    public Transform HitPoint { get => _hitPoint; }
    public Transform EndPoint { get => _endPoint; }
    public Vector3 ColliderSize { get => _colliderSize; }
    public LayerMask MonsterLayer { get => _monsterLayer; }
    public List<TempoAttackData> TempoAttackDatas { get => _tempoAttackDatas; }

    private void Awake()
    {
        _view = GetComponent<PlayerView>();

        _attack = new PlayerAttack(this);
        _controller = new PlayerController(this);

        _rb = GetComponent<Rigidbody>();
        _ani = GetComponentInChildren<Animator>();

        _stat.Initialize();
    }

    private void Start()
    {
        _attack.Initialize();
        _controller.Initialize();

        //�÷��̾� ����
        _stateStorage.Add(Define.PlayerState.NONE, new NoneState(this));
        _stateStorage.Add(Define.PlayerState.OVERLOAD, new OverloadState(this));
        _stateStorage.Add(Define.PlayerState.STUN, new StunState(this));
    }

    private void Update()
    {
        _stateStorage[_currentState]?.Stay();

        switch (_currentState)
        {
            case Define.PlayerState.STUN:
                //_attack.ChangeCurrentAttackState(Define.AttackState.FINISH);
                break;
            case Define.PlayerState.OVERLOAD:
            case Define.PlayerState.NONE:
                //_atkStateStorage[_curAtkState]?.Stay();
                _attack.Update();
                _controller.Update();

                break;
        }

    }

    public float GetTotalDamage(bool value = true)
    {
        if (value)
        {
            return _stat.Damage + _attack.CurrentAttackTempoData.maxDamage;
        }
        else
        {
            return _stat.Damage + _attack.CurrentAttackTempoData.minDamage;
        }   
    }

    public void TakeDamage(float value)
    {
        if (_stat.IsKnockedBack) return;

        _stat.Health -= value * ((100 - _stat.Defense) / 100);
        UpdateHealth();
    }

    //�˹� �Լ�
    public void Knockback(Vector2 knockbackDirection, float t = 0)
    {
        StartCoroutine(StartKnockBack(knockbackDirection, t));
    }
    // �˹� ����
    public IEnumerator StartKnockBack(Vector2 knockbackDirection, float t)
    {
        _stat.IsKnockedBack = true;
        GetComponent<Rigidbody>().AddForce(knockbackDirection, ForceMode.Impulse);

        yield return new WaitForSeconds(t);

        GetComponent<Rigidbody>().velocity = Vector2.zero;
        _stat.IsKnockedBack = false;
    }

    public void Heal(float value)
    {
        _stat.Health += value;
        UpdateHealth();
    }

    public void PowerUp(float value)
    {
        _stat.Damage += value;
    }

    // ����ȭ �������� Ȯ��(���׹̳ʰ� �ִ� ���׹̳��� ���� ��)
    public bool CheckOverload()
    {
        if (_stat.Stamina == _stat.MaxStamina)
        {
            return true;
        }
        return false;
    }

    #region View
    public void UpdateHealth()
    {
        _view.UpdateHpBar(_stat.Health / _stat.MaxHealth);
    }
    public void UpdateStamina()
    {
        print(_stat.Stamina);
        _view.UpdateStaminaBar(_stat.Stamina / _stat.MaxStamina);
    }
    public void UpdateUpgradeCount()
    {
        _view.UpdateUpgradeCountSlider(_attack.UpgradeCount);
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colliderSize);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
    }
}
