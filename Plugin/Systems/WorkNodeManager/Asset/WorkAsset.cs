using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NodeEditor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{
    public class PortAsset
    {
        public string PortName;
        public int NodeNo;
        public Type type;
        public PortAsset(string name, int no, Type type)
        {
            PortName = name;
            NodeNo = no;
            this.type = type;
        }
    }
    public class GroupAsset
    {
        public List<int> nodesNos;
        public string title;
        public GroupAsset()
        {
            nodesNos = new List<int>();
            title = "";
        }
        public GroupAsset(Group group)
        {
            nodesNos = new List<int>();
            title = group.title;//名字
            foreach (var node in group.containedElements)
            {
                if (node.GetType().IsSubclassOf(typeof(Node)))//继承自Node类
                {
                    Node val = node as Node;
                    //Debug.Log(val.title);
                    nodesNos.Add(int.Parse(val.name));
                }
            }
        }
    }
    public enum WorkAssetEnum
    {
        Public,
        Private
    }
    /// <summary>
    /// 图的数据,类似于一个程序的源代码
    /// </summary>
    [CreateAssetMenu(fileName = "NewWorkAsset", menuName = "Assets/ActivityObject/WorkAsset")]
    public class WorkAsset : SerializedScriptableObject
    {
        public WorkAssetEnum workAssetEnum = WorkAssetEnum.Public;
        /// <summary>
        /// 所有节点
        /// </summary>
        public List<NodeAsset> nodes = new List<NodeAsset>();
        public int beginNo = -1;
        public int endNo = -1;
        public int breakNo = -1;
        public int judgeNo = -1;
        public int EnvirNo = -1;//环境节点
        public List<GroupAsset> groups = new List<GroupAsset>();
        /// <summary>
        /// 一系列的行为节点
        /// </summary>
        [ReadOnly]
        public ObjectStruct root;
        [Button]
        public void SetObjectStruct(ObjectStruct obj)
        {
            root = obj;
            LoadUpdate.InitAssetObject();
            LoadUpdate.InitAlllObjects();
        }
        /// <summary>
        /// 相关节点的行为
        /// </summary>
        public Dictionary<string, ObjectStruct> relate = new Dictionary<string, ObjectStruct>();
        public List<Dictval> dictvals = new List<Dictval>();
        [Tooltip("专为此流程图可见的方法")]
        public MonoScript privateVal;
        public Type[] GetPrivateVal()
        {
            if (privateVal != null)
                return BasicFunction.GetTypesInNamespace(Assembly.GetExecutingAssembly(), privateVal.name + "PAct");
            return new Type[] { };
        }
        /// <summary>
        /// 生成节点源代码
        /// </summary>
        public void GenerateNodeSourceCode(DecisionNode node)
        {
        }
        public List<WorkAsset> PrivateWork;


        /// <summary>
        /// //结束整个项目返回-1,否则返回序号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int nextNode(NodeAsset node)
        {
            int nextNo = node.NextNode("succ");//根据字符串匹配下一个节点
            if (nextNo == endNo)
            {
                return -1;
            }
            else
            {
                return nextNo;
            }
        }
        public int nextNode(NodeAsset node, string val, WorkProcess workProcess)
        {
            //Debug.Log(name);
            int nextNo = node.NextNode(val);
            if (nextNo == endNo)//结束节点
            {
                //Debug.Log(workProcess.workAsset);
                workProcess.retVal.retString = node.output[val].PortName;
                var nextNode = workProcess.workAsset.nodes[nextNo];
                if (nextNode.inVal["obj"] != null)
                {
                    var parNo = nextNode.inVal["obj"].NodeNo;
                    workProcess.retVal.retVal = workProcess.workEnvir[workProcess.workAsset.nodes[parNo].HashCode(nextNode.inVal["obj"].PortName)];
                }
                else
                {
                    workProcess.retVal.retVal = null;
                }
                return -1;
            }
            else if (nextNo == breakNo)//中断节点
            {
                return -2;
            }
            else
            {
                return nextNo;
            }
        }
        public void Init()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].no = i;
            }
        }

    }
}