using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    //public class BreakNode : BaseNode
    //{
    //    public BreakNode()
    //    {
    //        style.backgroundColor = Color.black;
    //        AssetType = typeof(BreakNodeAsset);
    //        //nodeType = NodeType.endNode;
    //        title = "break";
    //        var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
    //        //����port��ʾ������
    //        inPort.portName = "bef";
    //        inPort.name = "bef";
    //        inPort.portColor = Color.green;
    //        inPort.portType = typeof(Node);
    //        inPort.allowMultiDrag = false;
    //        inputContainer.Add(inPort);
    //    }
    //    public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//������ĳ�ʼ��
    //    {
    //        var temp = base.GenerateNodeAsset(node, workAsset);
    //        temp.canExe = false;
    //        return temp;
    //    }
    //}
}