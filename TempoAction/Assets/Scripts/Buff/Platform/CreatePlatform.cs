using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlatform : MonoBehaviour
{
    [System.Serializable]
    private class buffInfoList
    {
        [Header("���� ���� ����Ʈ")]
        public List<Define.BuffInfo> infoList = new List<Define.BuffInfo>();
    }
    [Header("�÷��� ���� ����Ʈ")]
    [SerializeField] private List<buffInfoList> _platformList = new List<buffInfoList>();
    private List<BuffPlatform> _curPlatformList = new List<BuffPlatform>();

    [Header("������")]
    [SerializeField] private GameObject _platformPrefab;

    private float _width;

    [Header("�÷����� �ٲ�� �ð�")]
    [SerializeField] private float _changeTime; // �ٲ�� �ð�
    private float _timer;

    private int _index = 0;
    public int Index 
    {
        get => _index;
        set
        {
            _index = value;
            _index = _index % _platformList.Count;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _width = _platformPrefab.GetComponent<Renderer>().bounds.size.x;
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer >= _changeTime)
        {
            Change();
            _timer = 0;
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    private void Create() // �ʱ� �÷��� ���� �Լ�
    {
        int half = _platformList[_index].infoList.Count / 2;
        float posX = transform.position.x - (_width * half);

        for (int i = 0; i< _platformList[_index].infoList.Count; i++)
        {
            GameObject temp = Instantiate(_platformPrefab, new Vector3(posX, transform.position.y, transform.position.z), Quaternion.identity, transform);

            BuffPlatform platform = temp.GetComponent<BuffPlatform>();
            platform.Change(_platformList[_index].infoList[i]);

            _curPlatformList.Add(platform.GetComponent<BuffPlatform>());
            posX += _width;
        }

        Index++;
    }

    private void Change() // �÷��� ��ü �Լ�
    {
        for (int i = 0; i < _curPlatformList.Count; i++)
        {
            _curPlatformList[i].Change(_platformList[_index].infoList[i]);
        }

        Index++;
    }

    private void OnDrawGizmos()
    {
        if (_platformList.Count == 0) return;

        Vector3 size = _platformPrefab.GetComponent<Renderer>().bounds.size;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x * _platformList[0].infoList.Count, size.y, size.z));
    }
}
