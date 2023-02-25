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
    public Type type;//��Ӧproperty
    public string Name;
    public BaseElement(Type type,string Name, BaseNode baseNode)
    {
        this.type = type;
        this.Name = Name;
        this.baseNode = baseNode;
    }
    /// <summary>
    /// Ϊ��������ֵ
    /// </summary>
    /// <param name="val"></param>
    public abstract void SetVal(object val);

    public abstract object GetVal();
}
