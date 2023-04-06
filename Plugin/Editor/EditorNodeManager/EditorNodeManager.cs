using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NodeEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EditorNodeManager
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

    static void GenerateDecision(ObjectStruct objectStruct,ref List<SearchTreeEntry> entries,bool root=false)
    {
        Dictionary<string, List<Type>> keys = new Dictionary<string, List<Type>>();
        foreach (var v in objectStruct.decisionDict)
        {
            IdAttribute id = v.GetCustomAttribute<IdAttribute>();
            if (id != null)
            {
                if (keys.ContainsKey(id.name))
                {
                    keys[id.name].Add(v);
                }
                else
                {
                    keys.Add(id.name, new List<Type>());
                    keys[id.name].Add(v);
                }
            }
        }
        entries.Add(new SearchTreeGroupEntry(new GUIContent(objectStruct.name)) { level = 2 });
        foreach (var k in keys)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(k.Key)) { level = 3 });      //添加了一个二级菜单
            foreach (var v in k.Value)
            {
                if(!(v.GetCustomAttribute<IdAttribute>().privateEnum==PrivateEnum.Private&&root==false))
                entries.Add(new SearchTreeEntry(new GUIContent(v.Name)) { level = 4, userData = new GroupEntity(new DecisionPort(v.Name, v), "Decision") });
            }
        }
        if (objectStruct.script.GetClass() != null && objectStruct.script.GetClass().BaseType != null)
        {
            if(objectStruct.parent)
            {
                GenerateDecision(objectStruct.parent, ref entries);
            }
        }
    }

    static void GenerateBehavior(ObjectStruct objectStruct, ref List<SearchTreeEntry> entries, bool root = false)
    {
        Dictionary<string, List<Type>> keys = new Dictionary<string, List<Type>>();
        foreach (var v in objectStruct.behaviorDict)
        {
            IdAttribute id = v.GetCustomAttribute<IdAttribute>();
            if (id != null)
            {
                if (keys.ContainsKey(id.name))
                {
                    keys[id.name].Add(v);
                }
                else
                {
                    keys.Add(id.name, new List<Type>());
                    keys[id.name].Add(v);
                }
            }
        }
        entries.Add(new SearchTreeGroupEntry(new GUIContent(objectStruct.name)) { level = 2 });
        foreach (var k in keys)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(k.Key)) { level = 3 });      //添加了一个二级菜单
            foreach (var v in k.Value)
            {
                if (!(v.GetCustomAttribute<IdAttribute>().privateEnum == PrivateEnum.Private && root == false))
                    entries.Add(new SearchTreeEntry(new GUIContent(v.Name)) { level = 4, userData = new GroupEntity(new DecisionPort(v.Name, v), "Behavior") });
            }
        }
        if (objectStruct.script.GetClass() != null && objectStruct.script.GetClass().BaseType != null)
        {
            if (objectStruct.parent)
            {
                GenerateBehavior(objectStruct.parent, ref entries);
            }
        }
    }

    static void GenerateWork(ObjectStruct objectStruct, ref List<SearchTreeEntry> entries, bool root = false)
    {
        entries.Add(new SearchTreeGroupEntry(new GUIContent(objectStruct.name)) { level = 2 });
        foreach (var v in objectStruct.workAssetDict)
        {
            if (!(root == false&& v.workAssetEnum==PrivateEnum.Private))
                entries.Add(new SearchTreeEntry(new GUIContent(v.name)) { level = 3, userData = new GroupEntity(v, "WorkNode") });
        }
        if(objectStruct.parent)
        {
            GenerateWork(objectStruct.parent,ref entries);
        }
    }
    public static List<SearchTreeEntry> CreateSearchList()
    {
        var entries = new List<SearchTreeEntry>();
        if (WorkEditor.workAsset != null)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create")));                        //添加了一个一级菜单
            if (WorkEditor.workAsset.root != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("DecisionNode")) { level = 1 });      //添加了一个二级菜单
                GenerateDecision(WorkEditor.workAsset.root,ref entries,true);
            }
            ///////////////////////////////////////////////////////////////////工作节点
            if (WorkEditor.workAsset.root!=null&&WorkEditor.workAsset.root.workAssetDict!=null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("WorkNode")) { level = 1 });      //添加了一个二级菜单
                GenerateWork(WorkEditor.workAsset.root,ref entries,true);
            }
            entries.Add(new SearchTreeGroupEntry(new GUIContent("NeededNode")) { level = 1 });      //添加了一个二级菜单
            entries.Add(new SearchTreeEntry(new GUIContent("BeginNode")) { level = 2, userData = new GroupEntity(typeof(BeginNode), "BeginNode") });
            entries.Add(new SearchTreeEntry(new GUIContent("EndNode")) { level = 2, userData = new GroupEntity(typeof(EndNode), "EndNode") });
            /*******************************Group菜单的创建*******************************************/
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Group")) { level = 1 });                        //添加了一个一级菜单
            /*******************************Group菜单的创建*******************************************/
            entries.Add(new SearchTreeEntry(new GUIContent("NoteGroup")) { level = 2, userData = new GroupEntity("Note", "Group") });
            /**************************************OtherWorkNode**********************************************/
            if (WorkEditor.workAsset.relate != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("OtherWorkNode")) { level = 1 });      //添加了一个二级菜单
                foreach (var v in WorkEditor.workAsset.relate)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(v.Key)) { level = 2 });      //添加了一个二级菜单
                    foreach (var w in v.Value.workAssetDict)
                    {
                        if (w.workAssetEnum == PrivateEnum.Public)
                            entries.Add(new SearchTreeEntry(new GUIContent(w.name)) { level = 3, userData = new GroupEntity(w, "OtherWorkNode") });
                    }
                }
            }
            /*********************BehaviorNode*****************************/
            if (WorkEditor.workAsset.relate != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("BehaviorNode")) { level = 1 });      //添加了一个二级菜单
                foreach (var v in WorkEditor.workAsset.relate)//每一个节点都遍历
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(v.Key)) { level = 2 });      //添加了一个二级菜单
                    GenerateBehavior(v.Value, ref entries,true);
                }
            }
        }
        return entries;
    }
    /// <summary>
    /// 创建节点菜单
    /// </summary>
    /// <param name="nodetype"></param>
    /// <param name="localMousePosition"></param>
    /// <returns></returns>
    public static GraphElement CreateNodeMenu(GroupEntity nodetype,Vector2 localMousePosition)
    {
        if (nodetype.data == "WorkNode")
        {
            BaseNode node = new WorkNode((WorkAsset)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else if(nodetype.data== "OtherWorkNode")
        {
            BaseNode node = new OtherWorkNode((WorkAsset)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else if (nodetype.data == "Group")//Group
        {
            Group node = new Group();
            node.title = (String)nodetype.type;
            node.SetPosition(new Rect(localMousePosition, node.GetPosition().size));
            return node;
        }
        //else if ("Behavior" == nodetype.data || "Decision" == nodetype.data)//其他Node
        else if ("Decision" == nodetype.data)//其他Node
        {
            BaseNode node = new DecisionNode((DecisionPort)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else if ("Behavior" == nodetype.data)//其他Node
        {
            BaseNode node = new BehaviorNode((BehaviorPort)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else//begin或end
        {
            BaseNode node = Activator.CreateInstance((Type)nodetype.type) as BaseNode;
            node.NodePos(localMousePosition);
            return node;
        }
    }
}
