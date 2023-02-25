using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NodeEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class ElementFactory
{
    public static BaseElement GetElement(Type PropertyType,string Name,BaseNode baseNode)
    {
        if(PropertyType.Name=="WorkAsset")
        {
            Debug.Log(PropertyType.Name);
        }
        if (PropertyType == typeof(string))
        {
            return new StringElemnet(PropertyType, Name, baseNode);
        }
        else if (PropertyType == typeof(int))
        {
            return new IntegerElemnet(PropertyType, Name, baseNode);
        }
        else if (PropertyType == typeof(float))
        {
            return new FloatElemnet(PropertyType, Name, baseNode);
        }
        else if (PropertyType == typeof(bool))
        {
            return new BoolElemnet(PropertyType, Name, baseNode);
        }
        else if (PropertyType.IsEnum)
        {
            return new EnumElemnet(PropertyType, Name, baseNode);
        }
        //else if (PropertyType.GetGenericTypeDefinition() == typeof(List<>))//如果是List的话
        else if(BasicFunction.IsGenericList(PropertyType))
        {
            return new ListElement(PropertyType, Name, baseNode);
        }
        else//对象
        {
            return new ObjectElemnet(PropertyType, Name, baseNode);
        }
    }
}
