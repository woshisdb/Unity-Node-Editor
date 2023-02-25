using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using NodeEditor;

public class BeginNode : BaseNode
{
    public BeginNode()
    {
        style.backgroundColor = Color.green;
        AssetType = typeof(BeginNodeAsset);
        //nodeType = NodeType.beginNode;
        title = "begin";
        var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
        //����port��ʾ������
        outPort.portName = "succ";
        outPort.name = "succ";
        outPort.portColor = Color.red;
        outPort.portType = typeof(Node);
        outputContainer.Add(outPort);
    }
    public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//������ĳ�ʼ��
    {
        var temp=base.GenerateNodeAsset(node, workAsset);
        temp.canExe = false;
        return temp;
    }
}
