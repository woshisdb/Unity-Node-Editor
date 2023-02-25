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
        //设置port显示的名称
        outPort.portName = "succ";
        outPort.name = "succ";
        outPort.portColor = Color.red;
        outPort.portType = typeof(Node);
        outputContainer.Add(outPort);
    }
    public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
    {
        var temp=base.GenerateNodeAsset(node, workAsset);
        temp.canExe = false;
        return temp;
    }
}
