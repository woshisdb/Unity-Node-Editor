using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// 表示经由自己所做出的决策
    /// </summary>
    public abstract class Decision<T> : BaseAction<T> where T : ObjectManager
    {
    }
}