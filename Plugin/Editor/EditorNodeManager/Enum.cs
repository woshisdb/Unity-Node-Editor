using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GroupEntity
{
    public object type;//����ڵ�ı�Ҫ��Ϣ
    public string data;//�������ݽڵ������
    public GroupEntity(object type,string data)
    {
        this.type = type;
        this.data = data;
    }
}
//public enum VariableType
//{
//    public Type[] types = { typeof(GoTo),typeof(int) };
//}