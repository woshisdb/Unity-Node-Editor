using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// ��ʾ�����Լ��������ľ���
    /// </summary>
    public abstract class Decision<T> : BaseAction<T> where T : ObjectManager
    {
    }
}