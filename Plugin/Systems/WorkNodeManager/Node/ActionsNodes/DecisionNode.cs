using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{
    public class DecisionNode : ActionNode
    {
        public DecisionPort decisionPort;
        public DecisionNode(DecisionPort decisionPort) : base(Color.white, typeof(DecisionNodeAsset), decisionPort.DecisionName, Activator.CreateInstance(decisionPort.DecisionType) as BaseAction)
        {
            this.decisionPort = decisionPort;
        }

        public override void ReGeneratePort(List<string> Out, List<Dictval> InVal, List<Dictval> OutVal)
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

            base.ReGeneratePort(Out, InVal, OutVal);
        }

        public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//������ĳ�ʼ��
        {
            var temp = base.GenerateNodeAsset(node, workAsset);
            ((DecisionNodeAsset)temp).port = ((DecisionNode)node).decisionPort;
            temp.canExe = true;
            return temp;
        }
    }
}