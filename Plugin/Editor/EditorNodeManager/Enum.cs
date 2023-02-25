using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GroupEntity
{
    public object type;//创造节点的必要信息
    public string data;//创造数据节点的类型
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