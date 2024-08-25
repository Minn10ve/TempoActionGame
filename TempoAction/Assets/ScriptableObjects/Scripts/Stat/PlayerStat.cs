using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "ScriptableObjects/Stat/Player Stat")]
public class PlayerStat : Stat
{

    [Header("����")]
    [SerializeField] private float _jumpForce;// ���� ��

    [Header("�뽬")]
    [SerializeField] private float _dashDelay = 5f;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.2f;

    [Header("���¹̳�")]
    [SerializeField] private float _maxStamina;
    private float _stamina;

    [Header("����")]
    [SerializeField] private float _stunDelay; // ����ȭ �� ���ϱ��� �ɸ��� �ð�
    [SerializeField] private float _stunTime; // ���� ���� �ð�
 
    public bool IsKnockedBack { get; set; } = false;

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
                _isDead = true;
            }
            else if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }
        }
    }

    public float JumpForce { get => _jumpForce; set => _jumpForce = value; }
    public float DashDelay { get => _dashDelay; }
    public float DashDistance { get => _dashDistance; }
    public float DashDuration { get => _dashDuration; }
    public float MaxStamina { get => _maxStamina; }
    public float Stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            _stamina = value;

            if (_stamina >= _maxStamina)
            {
                _stamina = _maxStamina;
            }
            else if (_stamina < 0)
            {
                _stamina = 0;
            }
        }
    }
    public float StunDelay { get => _stunDelay; }// ���� ���� �ð�
    public float StunTime { get => _stunTime; }// ���� ���� �ð�

    public override void Initialize()
    {
        _health = _maxHealth;
        _stamina = 0;
    }

}
