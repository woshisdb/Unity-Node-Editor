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
    public static List<SearchTreeEntry> CreateSearchList()
    {
        var entries = new List<SearchTreeEntry>();
        if (WorkEditor.workAsset != null)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create")));                        //�����һ��һ���˵�
            /************************�ڵ�˵��Ĵ���*************************************************/
            //entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")) { level = 1 });
            /***************************�����˵��Ĵ���***************************************************/
            if (WorkEditor.workAsset.root != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("DecisionNode")) { level = 1 });      //�����һ�������˵�
                Dictionary<string, List<Type>> keys=new Dictionary<string, List<Type>>();
                foreach (var v in WorkEditor.workAsset.root.decisionDict) 
                {
                    IdAttribute id= v.GetCustomAttribute<IdAttribute>();
                    if(id!=null)
                    {
                        if(keys.ContainsKey(id.name))
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
                foreach(var k in keys)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(k.Key)) { level = 2 });      //�����һ�������˵�
                    foreach(var v in k.Value)
                    {
                        entries.Add(new SearchTreeEntry(new GUIContent(v.Name)) { level = 3, userData = new GroupEntity(new DecisionPort(v.Name, v), "Decision") });
                    }
                }
                if (WorkEditor.workAsset.privateVal != null)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent("PrivateDecision")) { level = 1 });      //�����һ�������˵�
                    foreach (var v in WorkEditor.workAsset.GetPrivateVal().Where(x=> IsAssignableToOpenGenericType(x, typeof(Decision<>) )))//ÿһ���ڵ㶼����
                    {
                        entries.Add(new SearchTreeEntry(new GUIContent(v.Name)) { level = 2, userData = new GroupEntity(new DecisionPort(v.Name, v), "Decision") });
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////�����ڵ�                                                                            //
            if (WorkEditor.workAsset.root!=null&&WorkEditor.workAsset.root.workAssetDict!=null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("WorkNode")) { level = 1 });      //�����һ�������˵�
                foreach (var v in WorkEditor.workAsset.root.workAssetDict)
                {
                    if(v.workAssetEnum==WorkAssetEnum.Public)
                        entries.Add(new SearchTreeEntry(new GUIContent(v.name)) { level = 2, userData = new GroupEntity(v, "WorkNode") });
                }

                entries.Add(new SearchTreeGroupEntry(new GUIContent("PrivateWorkNode")) { level = 1 });      //�����һ�������˵�
                foreach (var v in WorkEditor.workAsset.PrivateWork)
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(v.name)) { level = 2, userData = new GroupEntity(v, "WorkNode") });
                }

            }

            

            entries.Add(new SearchTreeGroupEntry(new GUIContent("NeededNode")) { level = 1 });      //�����һ�������˵�
            entries.Add(new SearchTreeEntry(new GUIContent("BeginNode")) { level = 2, userData = new GroupEntity(typeof(BeginNode), "BeginNode") });
            entries.Add(new SearchTreeEntry(new GUIContent("EndNode")) { level = 2, userData = new GroupEntity(typeof(EndNode), "EndNode") });
            //entries.Add(new SearchTreeEntry(new GUIContent("BreakNode")) { level = 2, userData = new GroupEntity(typeof(BreakNode), "BreakNode") });
            //entries.Add(new SearchTreeEntry(new GUIContent("JudgeNode")) { level = 2, userData = new GroupEntity(typeof(JudgeNode), "JudgeNode") });
            /*******************************Group�˵��Ĵ���*******************************************/
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Group")) { level = 1 });                        //�����һ��һ���˵�
            /*******************************Group�˵��Ĵ���*******************************************/
            entries.Add(new SearchTreeEntry(new GUIContent("NoteGroup")) { level = 2, userData = new GroupEntity("Note", "Group") });
            /************************************************************************************/
            if (WorkEditor.workAsset.relate != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("OtherWorkNode")) { level = 1 });      //�����һ�������˵�
                foreach (var v in WorkEditor.workAsset.relate)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(v.Key)) { level = 2 });      //�����һ�������˵�
                    foreach (var w in v.Value.workAssetDict)
                    {
                        if (w.workAssetEnum == WorkAssetEnum.Public)
                            entries.Add(new SearchTreeEntry(new GUIContent(w.name)) { level = 3, userData = new GroupEntity(w, "OtherWorkNode") });
                    }
                }
            }
            /**************************************************/
            if (WorkEditor.workAsset.relate != null)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent("BehaviorNode")) { level = 1 });      //�����һ�������˵�
                foreach (var v in WorkEditor.workAsset.relate)//ÿһ���ڵ㶼����
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(v.Key)) { level = 2 });      //�����һ�������˵�
                    foreach (var u in v.Value.behaviorDict)
                    {
                        entries.Add(new SearchTreeEntry(new GUIContent(u.Name)) { level = 3, userData = new GroupEntity(new BehaviorPort(v.Key, u), "Behavior") });
                    }
                }
            }
        }
        return entries;
    }
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
        //else if ("Behavior" == nodetype.data || "Decision" == nodetype.data)//����Node
        else if ("Decision" == nodetype.data)//����Node
        {
            BaseNode node = new DecisionNode((DecisionPort)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else if ("Behavior" == nodetype.data)//����Node
        {
            BaseNode node = new BehaviorNode((BehaviorPort)nodetype.type);
            node.NodePos(localMousePosition);
            return node;
        }
        else//begin��end
        {
            BaseNode node = Activator.CreateInstance((Type)nodetype.type) as BaseNode;
            node.NodePos(localMousePosition);
            return node;
        }
    }
}
