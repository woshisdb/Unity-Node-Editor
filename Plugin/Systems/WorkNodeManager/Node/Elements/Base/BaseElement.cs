using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NodeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseElement:VisualElement
{
    public BaseNode baseNode;
    public Type type;//对应property
    public string Name;
    public BaseElement(Type type,string Name, BaseNode baseNode)
    {
        this.type = type;
        this.Name = Name;
        this.baseNode = baseNode;
    }
    /// <summary>
    /// 为对象设置值
    /// </summary>
    /// <param name="val"></param>
    public abstract void SetVal(object val);

    public abstract object GetVal();
}
