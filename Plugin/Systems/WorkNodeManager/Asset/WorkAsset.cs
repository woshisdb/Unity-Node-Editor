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
            title = group.title;//����
            foreach (var node in group.containedElements)
            {
                if (node.GetType().IsSubclassOf(typeof(Node)))//�̳���Node��
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
    /// ͼ������,������һ�������Դ����
    /// </summary>
    [CreateAssetMenu(fileName = "NewWorkAsset", menuName = "Assets/ActivityObject/WorkAsset")]
    public class WorkAsset : SerializedScriptableObject
    {
        public WorkAssetEnum workAssetEnum = WorkAssetEnum.Public;
        /// <summary>
        /// ���нڵ�
        /// </summary>
        public List<NodeAsset> nodes = new List<NodeAsset>();
        public int beginNo = -1;
        public int endNo = -1;
        public int breakNo = -1;
        public int judgeNo = -1;
        public int EnvirNo = -1;//�����ڵ�
        public List<GroupAsset> groups = new List<GroupAsset>();
        /// <summary>
        /// һϵ�е���Ϊ�ڵ�
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
        /// ��ؽڵ����Ϊ
        /// </summary>
        public Dictionary<string, ObjectStruct> relate = new Dictionary<string, ObjectStruct>();
        public List<Dictval> dictvals = new List<Dictval>();
        [Tooltip("רΪ������ͼ�ɼ��ķ���")]
        public MonoScript privateVal;
        public Type[] GetPrivateVal()
        {
            if (privateVal != null)
                return BasicFunction.GetTypesInNamespace(Assembly.GetExecutingAssembly(), privateVal.name + "PAct");
            return new Type[] { };
        }
        /// <summary>
        /// ���ɽڵ�Դ����
        /// </summary>
        public void GenerateNodeSourceCode(DecisionNode node)
        {
        }
        public List<WorkAsset> PrivateWork;


        /// <summary>
        /// //����������Ŀ����-1,���򷵻����
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int nextNode(NodeAsset node)
        {
            int nextNo = node.NextNode("succ");//�����ַ���ƥ����һ���ڵ�
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
            if (nextNo == endNo)//�����ڵ�
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
            else if (nextNo == breakNo)//�жϽڵ�
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