using System.Collections;
using System.Collections.Generic;
using NodeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


//public class JudgeNode : BaseNode
//{
//    public JudgeNode()
//    {
//        style.backgroundColor = Color.cyan;
//        AssetType = typeof(JudgeNodeAsset);
//        //nodeType = NodeType.beginNode;
//        title = "judge";
//        var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
//        //设置port显示的名称
//        outPort.portName = "succ";
//        outPort.name = "succ";
//        outPort.portColor = Color.red;
//        outPort.portType = typeof(Node);
//        outputContainer.Add(outPort);
//        //ObjectField objectField = new ObjectField();
//        //objectField.objectType = typeof(int);
//        //objectField.name="Object";
//        //contentContainer.Add(objectField);//内容的东西
//    }
//    public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
//    {
//        var temp = base.GenerateNodeAsset(node, workAsset);
//        temp.canExe = false;
//        return temp;
//    }
//}
