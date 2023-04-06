using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// 对一类交互对象的行为进行管理的类
    /// </summary>
    [CreateAssetMenu(fileName = "New ObjectStruct", menuName = "Assets/ActivityObject/ObjectStruct")]
    public class ObjectStruct : SerializedScriptableObject
    {
        public static bool IsAssignableToOpenGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();
            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;
            Type baseType = givenType.BaseType;
            if (baseType == null) return false;
            return IsAssignableToOpenGenericType(baseType, genericType);
        }
        [ReadOnly]
        public ObjectStruct parent;
        public MonoScript script;
        public string sourceName { get => script.GetClass().Name; }//变量的名字
        public List<Type> behaviorDict;//活动字典,和这个活动相互转化的活动
        public List<Type> decisionDict;//活动字典,和这个活动相互转化的活动
        public List<WorkAsset> workAssetDict;//所有和它相关的活动
        [Button("Init")]
        public void Init()//生成所有attrribute相关的行为
        {
            Type type = Assembly.GetExecutingAssembly().GetType(sourceName);
            behaviorDict = new List<Type>();
            decisionDict = new List<Type>();
            Type[] types = BasicFunction.GetTypesInNamespace(Assembly.GetExecutingAssembly(), sourceName + "Act");
            foreach (Type t in types)
            {
                if (IsAssignableToOpenGenericType(t, typeof(Behavior<>)))
                {
                    behaviorDict.Add(t);
                }
                else if (IsAssignableToOpenGenericType(t, typeof(Decision<>)))//它是Behave
                {
                    decisionDict.Add(t);
                }
            }
            workAssetDict.Clear();
            foreach (var i in AssetDatabase.FindAssets("t:" + typeof(WorkAsset).Name).Select(guid => AssetDatabase.LoadAssetAtPath<WorkAsset>(AssetDatabase.GUIDToAssetPath(guid))).ToList().FindAll(t => t is WorkAsset && t.root == this))
            {
                i.Init();
                if (workAssetDict.Contains(i) == false)
                    workAssetDict.Add(i);
            }
            if (type != null&& type.BaseType != null)
            {
                Init(type.BaseType.Name);
                parent= AssetDatabase.FindAssets("t:" + typeof(ObjectStruct).Name).Select(guid => AssetDatabase.LoadAssetAtPath<ObjectStruct>(AssetDatabase.GUIDToAssetPath(guid))).ToList().Find(t => t is ObjectStruct && t.sourceName == type.BaseType.Name);
            }
        }
        private void Init(string sourceName)//生成所有attrribute相关的行为
        {
            ObjectStruct faobj = AssetDatabase.FindAssets("t:" + typeof(ObjectStruct).Name).Select(guid => AssetDatabase.LoadAssetAtPath<ObjectStruct>(AssetDatabase.GUIDToAssetPath(guid))).ToList().Find(t => t is ObjectStruct && t.sourceName == sourceName);//FindAll(t => t is ObjectStruct && t.root == this);
            Type type = Assembly.GetExecutingAssembly().GetType(sourceName);
            if (type != null)
            {
                //List<Type> types = type.GetNestedTypes().ToList();
                Type[] types = BasicFunction.GetTypesInNamespace(Assembly.GetExecutingAssembly(), sourceName + "Act");
                //Debug.Log(types.Count);
                foreach (var t in types)
                {
                    if (IsAssignableToOpenGenericType(t, typeof(Behavior<>)))
                    {
                        behaviorDict.Add(t);
                    }
                    else if (IsAssignableToOpenGenericType(t, typeof(Decision<>)))//它是Behave
                    {
                        decisionDict.Add(t);
                    }
                }
            }
            if (faobj != null)//初始化对应的objectstruct；
            {
                faobj.Init();
                foreach (var i in faobj.workAssetDict)
                {
                    if (workAssetDict.Contains(i) == false)
                        workAssetDict.Add(i);
                }
            }
            if (type != null && type.BaseType != null)
            {
                Init(type.BaseType.Name);
            }
        }
    }
}