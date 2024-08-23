using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Monster : MonoBehaviour
{
    private MonsterView _view;

    protected Rigidbody _rb;
    protected Transform _player;
    [SerializeField] protected MonsterStat _stat;

    [SerializeField] protected LayerMask _playerLayer;
    [SerializeField] protected LayerMask _wallLayer;
    
    protected float _direction = 1; // ���Ͱ� �ٶ󺸴� ����

    protected bool _canKnockback; // true =  �˹� ������ ����, false = �˹� �Ұ����� ����
    private float _knockBackTimer = 0;
    public Action OnKnockback;

    public bool IsKnockBack { get; set; } = false;
    public bool IsStunned { get; set; } = false;

    public Action OnPointTempo;


    public Rigidbody Rb { get { return _rb; } }
    public Transform Player { get => _player; }
    public MonsterStat Stat { get { return _stat; } }
    public LayerMask PlayerLayer { get => _playerLayer; }
    public LayerMask WallLayer { get => _wallLayer; }
    public bool CanKnockback { get => _canKnockback; }
    public float Direction
    {
        get => _direction;
        set
        {
            if (value > 0)
            {
                value = 1;
            }
            else if (value < 0)
            {
                value = -1;
            }

            if (_direction != value)
            {
                Flip(value);
            }

            _direction = value;
        }
    }

    private void Awake()
    {
        _view = GetComponent<MonsterView>();

        Initialize();
    }

    protected abstract void Initialize();

    // ���� �Լ�
    public void Flip(float value) 
    {
        transform.GetChild(0).localScale = new Vector3(value, 1, 1);
    }

    public void TakeDamage(float value, bool isPointTempo = false)
    {
        _stat.Health -= value * ((100 - _stat.Defense) / 100);

        if (isPointTempo)
        {
            OnPointTempo?.Invoke();
        }

        UpdateHealth();
    }

    // �˹� ��ġ�� ����ϴ� �̺�Ʈ �Լ�
    public void KnockBack(Vector2 hitPoint, Vector2 endPoint)
    {
        Vector2 distance = (Vector2)transform.position - hitPoint;
        Vector2 ep = endPoint + distance;

        _knockBackTimer = 0;
        IsKnockBack = true;


        transform.DOMove(ep, 0.2f).OnComplete(() =>
        {
           IsKnockBack = false;

            if (!IsStunned)
            {
                StartCoroutine(Stun());

            }

        });


    }

    // �˹� �� ��� ���� 
    private IEnumerator Stun()
    {
        IsStunned = true;
        while (_knockBackTimer < 0.5f)
        {
            _knockBackTimer += Time.deltaTime;

            yield return null;
        }
        IsStunned = false;

        OnKnockback?.Invoke();
    }

    #region View
    public void UpdateHealth()
    {
        _view.UpdateHpBar(_stat.Health / _stat.MaxHealth);
    }
    #endregion

}
