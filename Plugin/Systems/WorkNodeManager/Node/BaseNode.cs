using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{

    public class BaseNode : Node
    {
        public Type AssetType;
        public BaseAction nowAction;
        public BaseNode()
        {
        }
        public void NodePos(Vector2 pos)
        {
            SetPosition(new Rect(pos, GetPosition().size));
        }
        public virtual object GenerateAsset()
        {
            return null;
        }
        //public NodeType nodeType;
        //���ڽڵ�������Դ
        public virtual NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//������ĳ�ʼ��
        {
            NodeAsset NodeAsset = (NodeAsset)ScriptableObject.CreateInstance(AssetType);//������Դ�ļ�
                                                                                        //NodeAsset.Init(node, workAsset);
            NodeAsset.pos = node.GetPosition();
            NodeAsset.type = node.GetType();
            //NodeAsset.input = new Dictionary<string, List<PortAsset>>();
            NodeAsset.output = new Dictionary<string, PortAsset>();
            NodeAsset.inVal = new Dictionary<string, PortAsset>();
            NodeAsset.fields = new Dictionary<string, object>();
            //NodeAsset.outVal = new Dictionary<string, List<PortAsset>>();
            //����ڵ�
            for (int i = 0; i < node.inputContainer.childCount; i++)
            {
                Port temp = (Port)node.inputContainer.ElementAt(i);
                if (temp.portType == typeof(Node))
                {
                    //NodeAsset.input.Add(temp.portName, new List<PortAsset>());
                }
                else//ֵ�ڵ�
                {
                    NodeAsset.inVal.Add(temp.portName, null);
                }
            }
            //����ڵ�
            for (int i = 0; i < node.outputContainer.childCount; i++)
            {
                Port temp = (Port)node.outputContainer.ElementAt(i);
                if (temp.portType == typeof(Node))
                {
                    NodeAsset.output.Add(temp.portName, null);
                }
                else
                {
                    //NodeAsset.outVal.Add(temp.portName, new List<PortAsset>());
                }
            }
            SaveField(NodeAsset);
            //AssetDatabase.AddObjectToAsset(NodeAsset, workAsset);
            return NodeAsset;
        }
        public virtual void ReGeneratePort(List<string> Out, List<Dictval> InVal, List<Dictval> OutVal)
        {
            outputContainer.Clear();
            inputContainer.Clear();
            var befPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //����port��ʾ������
            befPort.portName = "bef";
            befPort.name = "bef";
            befPort.portColor = Color.green;
            befPort.portType = typeof(Node);
            inputContainer.Add(befPort);

            var objPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //����port��ʾ������
            objPort.portName = "obj";
            objPort.name = "obj";
            objPort.portColor = Color.cyan;
            objPort.portType = typeof(ObjectManager);
            inputContainer.Add(objPort);

            foreach (string val in Out)
            {
                var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                //����port��ʾ������
                outPort.portName = val;
                outPort.name = val;
                outPort.portType = typeof(Node);
                outPort.portColor = Color.red;
                outputContainer.Add(outPort);
            }
            foreach (var val in InVal)//����ڵ�ֵ
            {
                var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
                //����port��ʾ������
                inPort.portName = val.name;
                inPort.name = val.name;
                Color x;
                ColorUtility.TryParseHtmlString("#282828", out x);//Color.FromArgb(0x282828);//Color.cyan;
                inPort.portColor = x;
                inPort.portType = val.type;
                inputContainer.Add(inPort);
            }
            foreach (var val in OutVal)//����ڵ�ֵ
            {
                var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
                //����port��ʾ������
                outPort.portName = val.name;
                outPort.name = val.name;
                outPort.portType = val.type;
                //outPort.portColor = Color.blue;
                Color x;
                ColorUtility.TryParseHtmlString("#282828", out x);//Color.FromArgb(0x282828);//Color.cyan;
                outPort.portColor = x;
                outputContainer.Add(outPort);
            }
            RefreshExpandedState();
        }



        /// <summary>
        /// ��������ȥ����Field
        /// </summary>
        /// <param name="now"></param>
        public void SetField(BaseAction now)
        {
            foreach (FieldInfo pi in now.GetType().GetFields())
            {
                if (pi.GetCustomAttribute<ShowAttribute>() != null)
                    contentContainer.Add(ElementFactory.GetElement(pi.FieldType, pi.Name, this));
            }
        }
        /// <summary>
        /// ��ÿ��node���ݣ�����ÿAsset������ȥ����Field
        /// </summary>
        /// <param name="now"></param>
        public void LoadField(NodeAsset now)
        {
            //Debug.Log(now.name);
            foreach (var t in now.fields)
            {
                //var x in contentContainer.Children()
                var temp = contentContainer.Q<BaseElement>(t.Key);
                temp.SetVal(t.Value);
                nowAction.GetType().GetField(temp.name).SetValue(nowAction, t.Value);
            }
            var res = nowAction.UpdateOutPort();
            if (res != null && (res.K1 != null || res.K2 != null || res.K3 != null))
            {
                if (res.K1 == null)
                {
                    res.K1 = nowAction.Out;
                }
                if (res.K2 == null)
                {
                    res.K2 = nowAction.OutVal;
                }
                if (res.K3 == null)
                {
                    res.K3 = nowAction.InVal;
                }
                ReGeneratePort(res.K1, res.K3, res.K2);
            }
        }
        /// <summary>
        /// ����ÿһ��Fieldȥ��������
        /// </summary>
        /// <param name="now"></param>
        public void SaveField(NodeAsset now)
        {
            if (nowAction != null)
                foreach (FieldInfo pi in nowAction.GetType().GetFields())
                {
                    if (pi.GetCustomAttribute<ShowAttribute>() != null)
                        now.fields.Add(pi.Name, contentContainer.Q<BaseElement>(pi.Name).GetVal());
                }
        }
    }
}