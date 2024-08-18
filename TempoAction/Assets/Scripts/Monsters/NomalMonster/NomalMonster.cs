using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NomalMonster : Monster
{
    [System.Serializable]
    public class PerceptionRange
    {
        [Range(0, 100)]
        public int range_Ratio;
        public float gaugeIncrementPerSecond;
        public Color color;
    }

    [SerializeField] private float _moveRange;
    public float MoveRange { get => _moveRange; }

    [SerializeField] private float _perceptionDistance;
    public float PerceptionDistance { get => _perceptionDistance; }

    [SerializeField] private float _perceptionAngle;
    public float PerceptionAngle { get => _perceptionAngle; }

    [SerializeField] private List<PerceptionRange> _perceptionRanges = new List<PerceptionRange>();
    public List<PerceptionRange> PerceptionRanges { get => _perceptionRanges; }

    [SerializeField] private float _maxAggroGauge;
    public float MaxAggroGauge { get => _maxAggroGauge; }

    [SerializeField] private float _aggroGauge = 0;
    public float AggroGauge
    {
        get => _aggroGauge;
        set
        {

            if (value < 0)
            {
                _aggroGauge = 0;
            }
            else
            {
                _aggroGauge = value;

            }

            if (_aggroGauge > _maxAggroGauge)
            {
                _aggroGauge = _maxAggroGauge;
            }

        }
    }

    private Vector2 _spawnPoint;
    public Vector2 SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }

    [SerializeField] private Transform _hitPoint;
    [SerializeField] private Vector3 _colSize;


    private Define.PerceptionType _curPerceptionState;
    public Define.PerceptionType CurPerceptionState
    {
        get
        {
            return _curPerceptionState;
        }
        set
        {
            if (_perceptionStateStorage.ContainsKey(_curPerceptionState))
            {
                _perceptionStateStorage[_curPerceptionState]?.Exit();
            }
            _curPerceptionState = value;
            _perceptionStateStorage[_curPerceptionState]?.Enter();
        }
    }

    private Dictionary<Define.PerceptionType, NomalMonster_State> _perceptionStateStorage = new Dictionary<Define.PerceptionType, NomalMonster_State>();

    [SerializeField] private Canvas _nomalMonsterCanvas;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _rb = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>().transform;

        _nomalMonsterCanvas.worldCamera = Camera.main;
    }
    private void Start()
    {
        _perceptionStateStorage.Add(Define.PerceptionType.PATROL, new NomalMonster_Patrol(this));
        _perceptionStateStorage.Add(Define.PerceptionType.BOUNDARY, new NomalMonster_Boundary(this));
        _perceptionStateStorage.Add(Define.PerceptionType.DETECTIONM, new NomalMonster_Detectionm(this));

        CurPerceptionState = Define.PerceptionType.PATROL;

        _canKnockback = true;

        _stat.Hp = _stat.MaxHp;

        OnKnockback += () =>
        {
            float dir = _player.position.x - transform.position.x;
            Direction = dir;
        };

        
    }

    private void Update()
    {

        CheckPerceptionState(); // ������ ���� ��Ű�� �Լ�
        UpdatePerceptionState(); // ������ Ȯ�� �� �ν� ���� ������Ʈ

        if (_perceptionStateStorage[_curPerceptionState].IsEntered)
        {
            _perceptionStateStorage[_curPerceptionState]?.Stay();
        }

    }



    private void CheckPerceptionState() // ��ä�� �ȿ� �÷��̾ �ִ��� Ȯ��
    {
        // ���������� �÷��̾�� ���ϴ� ���� ���

        Vector3 playerPos = new Vector3(_player.position.x, _player.position.y + 0.5f, _player.position.z);
        Vector3 directionToPlayer = playerPos - transform.position;
        directionToPlayer.z = 0; // ����(z��)�� ������� ����

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(new Vector3(_direction, 0, 0), directionToPlayer);

        if (CurPerceptionState == Define.PerceptionType.DETECTIONM)
        {
            if (distanceToPlayer > _perceptionDistance)
            {
                AggroGauge -= 20 * Time.deltaTime;
            }
        }
        else
        {
            if (distanceToPlayer > _perceptionDistance || angleToPlayer > _perceptionAngle / 2.0f) // �÷��̾ ��ä���� ������ ���� ���� ��
            {
                AggroGauge -= 20 * Time.deltaTime;
            }
            else // �÷��̾ ��ä���� ������ ���� ���� ��
            {
                // ������ ����
                if (distanceToPlayer <= _perceptionDistance * ((float)_perceptionRanges[0].range_Ratio / 100.0f))
                {
                    AggroGauge += _perceptionRanges[0].gaugeIncrementPerSecond * Time.deltaTime;
                }
                else if (distanceToPlayer <= _perceptionDistance * ((float)_perceptionRanges[1].range_Ratio / 100.0f))
                {
                    AggroGauge += _perceptionRanges[1].gaugeIncrementPerSecond * Time.deltaTime;
                }
                else if (distanceToPlayer <= _perceptionDistance * ((float)_perceptionRanges[2].range_Ratio / 100.0f))
                {
                    AggroGauge += _perceptionRanges[2].gaugeIncrementPerSecond * Time.deltaTime;
                }

            }
        }

        //UIManager.Instance.GetUI<Image>("PerceptionGauge").fillAmount = _aggroGauge / _maxAggroGauge;
    }


    private void UpdatePerceptionState()
    {
        if (_aggroGauge == 0)
        {
            if (CurPerceptionState != Define.PerceptionType.PATROL)
            {
                CurPerceptionState = Define.PerceptionType.PATROL;
            }
        }
        else if (_aggroGauge == 100)
        {
            if (CurPerceptionState != Define.PerceptionType.DETECTIONM)
            {
                CurPerceptionState = Define.PerceptionType.DETECTIONM;
            }
        }
        else
        {
            if (CurPerceptionState != Define.PerceptionType.BOUNDARY)
            {
                CurPerceptionState = Define.PerceptionType.BOUNDARY;
            }
        }
    }

    public void Attack()
    {
        bool isHit = Physics.CheckBox(_hitPoint.position, _colSize/2, _hitPoint.rotation, _playerLayer);

        if (isHit)
        {
            print("����");
            _player.GetComponent<Player>().Stat.TakeDamage(_stat.AttackDamage);
        }

    }

  /*  public void Rotate()
    {

        print("dddd");
        Vector3 direction = _player.transform.position - transform.position;

        Vector3 cross = Vector3.Cross(Vector3.forward, direction);

        if (cross.y > 0)
        {
            Direction = 1;
        }
        else if (cross.y < 0)
        {
            Direction = -1;
        }
    }*/


   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_hitPoint.position, _colSize);

        if (_spawnPoint != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(_spawnPoint.x - _moveRange, transform.position.y, 0), new Vector3(_spawnPoint.x + _moveRange, transform.position.y, 0));
        }
    }

    private void OnDrawGizmosSelected()
    {
        int _segments = 10;

        for (int j = _perceptionRanges.Count - 1; j >= 0; j--)
        {
            Gizmos.color = _perceptionRanges[j].color;

            // ���� ������ �� ���� ����
            float halfAngle = _perceptionAngle / 2.0f;
            float startAngle = -halfAngle;
            float endAngle = halfAngle;

            // �߽����������� ����
            Vector3 startPosition = transform.position;

            // ���� ���� ���
            float angleStep = _perceptionAngle / _segments;

            float length = _perceptionDistance * ((float)_perceptionRanges[j].range_Ratio / 100.0f);

            // ��ä���� �׸��� ���� ������ ���
            Vector3 firstPoint = startPosition + Quaternion.Euler(0, 0, startAngle) * new Vector3(_direction, 0, 0) * length;
            Vector3 lastPoint = startPosition + Quaternion.Euler(0, 0, endAngle) * new Vector3(_direction, 0, 0) * length;

            Gizmos.DrawLine(startPosition, firstPoint);

            // ������ ���鼭 ��ä���� �׸���
            for (int i = 1; i <= _segments; i++)
            {
                float currentAngle = startAngle + i * angleStep;
                Vector3 nextPoint = startPosition + Quaternion.Euler(0, 0, currentAngle) * new Vector3(_direction, 0, 0) * length;

                // ���� �׸���

                Gizmos.DrawLine(firstPoint, nextPoint);

                // ���� ���� ���� ������ ����
                firstPoint = nextPoint;
            }

            // ������ ���� �߽����� �׸���
            Gizmos.DrawLine(startPosition, lastPoint);
        }


    }

}
