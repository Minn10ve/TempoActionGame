using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : Singleton<BuffManager>
{

    [SerializeField] private List<BuffData> _buffStorage = new List<BuffData>();

    private List<BuffData> _curBuffList = new List<BuffData>();
    private List<BuffData> _removeBuffList = new List<BuffData>();


    private void Update()
    {

        foreach (BuffData data in _curBuffList)
        {
            data.Stay();
        }

        if (_removeBuffList.Count <= 0) return;

        foreach (BuffData data in _removeBuffList)
        {
            _curBuffList.Remove(data);
        }

        _removeBuffList.Clear();
    }

    public void AddBuff(Define.BuffInfo buff) // ���� �߰�
    {

        if (FindCurBuff(buff) != null) // ���� ���� ������ �ִ��� Ȯ��
        {
            RemoveBuff(buff); // ���� ���� ����
        }

        BuffData buffData = FindBuff(buff);

        if (buffData == null) return;
        
        buffData.Enter();
        _curBuffList.Add(buffData);
    }

    public void RemoveBuff(Define.BuffInfo buff) // ���� �߰�
    {
        BuffData buffData = FindCurBuff(buff);

        if (buffData == null) return;

        buffData.Exit();
        _removeBuffList.Add(buffData);
    }

    // ���� ã��
    public BuffData FindBuff(Define.BuffInfo info) 
    {
        foreach (BuffData buff in _buffStorage)
        {
            if (buff.info == info)
            {
                return buff;
            }
        }

        return null;
    }


    private BuffData FindCurBuff(Define.BuffInfo info)
    {
        foreach (BuffData buff in _curBuffList)
        {
            if (buff.info == info)
            {
                return buff;
            }
        }

        return null;
    }
}