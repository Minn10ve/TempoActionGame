using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabStorage;  // �̸� ������ ��ü�� ������
    [SerializeField] private int _initialSize = 10;  // �ʱ� ������ ��ü�� ��

    private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    void Awake()
    {
        foreach (GameObject prefab in _prefabStorage)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            GameObject poolObj = new GameObject(prefab.name + "Pool");
            poolObj.transform.SetParent(transform);

            for (int i = 0; i < _initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolObj.transform);

                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            pool.Add(prefab.name, queue);
        }
       
    }

    // ��ü�� Ǯ���� ��������
    public GameObject Spawn(string name, float t = 0, Transform parent = null)
    {
        GameObject obj;

        if (pool[name].Count > 0)
        {
            obj = pool[name].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            // Ǯ�� ��ü�� ������ ���� ����
            obj = Instantiate(Find(name), transform);
            Reset(obj);
        }

        if (parent)
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
        }

        if (t != 0)
        {
            StartCoroutine(ReturnObj(obj, t));
        }

        return obj;
    }

    // ��ü�� Ǯ�� ��ȯ�ϱ�
    public void Remove(GameObject obj)
    {
        Reset(obj);
        obj.SetActive(false);
        pool[RemoveClone(obj.name)].Enqueue(obj);
    }

    private IEnumerator ReturnObj(GameObject obj, float t)
    {
        yield return new WaitForSeconds(t);
        Remove(obj);

    }

    private void Reset(GameObject obj)
    {
        string poolName = RemoveClone(obj.name) + "Pool"; 

        obj.transform.SetParent(FindPool(poolName));
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }

    private GameObject Find(string name)
    {
        foreach (GameObject prefab in _prefabStorage)
        {
            if (prefab.name == name)
            {
                return prefab;
            }
        }

        return null;
    }

    private Transform FindPool(string name)
    {
        foreach (Transform pool in transform)
        {
            if (pool.name == name)
            {
                return pool;
            }
        }

        return transform;
    }

    private string RemoveClone(string name)
    {
        int index = name.IndexOf("(Clone)");
        if (index > 0)
            name = name.Substring(0, index);

        return name;
    }
}
