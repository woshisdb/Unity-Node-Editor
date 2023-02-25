using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    public class OtherWorkNode : BaseNode
    {
        public WorkAsset index;//保存的是对象
        public OtherWorkNode(WorkAsset index)
        {
            style.backgroundColor = Color.Lerp(Color.cyan, Color.green, 0.5f);
            AssetType = typeof(OtherWorkNodeAsset);
            this.index = index;
            this.title = index.root.name + ":" + index.name;

            var endNode = index.nodes[index.endNo] as EndNodeAsset;
            if (endNode.ret == null || endNode.ret.Count == 0)
            {
                var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                //设置port显示的名称
                outPort.portName = "succ";
                outPort.name = "succ";
                outPort.portColor = Color.red;
                outPort.portType = typeof(Node);
                outputContainer.Add(outPort);
            }
            else
            {
                foreach (var x in endNode.ret)
                {
                    var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                    //设置port显示的名称
                    outPort.portName = x;
                    outPort.name = x;
                    outPort.portColor = Color.red;
                    outPort.portType = typeof(Node);
                    outputContainer.Add(outPort);
                }
            }

            //var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            ////设置port显示的名称
            //outPort.portName = "succ";
            //outPort.name = "succ";
            //outPort.portColor = Color.red;
            //outPort.portType = typeof(Node);
            //outputContainer.Add(outPort);
            if (index.breakNo != -1)
            {
                var expPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                expPort.portName = "break";
                expPort.name = "break";//发生预期外事件而终止
                expPort.portType = typeof(Node);
                expPort.portColor = Color.red;
                outputContainer.Add(expPort);
            }

            var retVPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            retVPort.portName = "obj";
            retVPort.name = "obj";
            retVPort.portColor = Color.cyan;
            retVPort.portType = typeof(object);
            outputContainer.Add(retVPort);

            foreach (var val in index.dictvals)//输入节点值
            {
                var i = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
                //设置port显示的名称
                i.portName = val.name;
                i.name = val.name;
                i.portColor = Color.cyan;
                i.portType = val.type;
                inputContainer.Add(i);
            }

            /////////////////输入节点///////////////////
            var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            inPort.portName = "bef";
            inPort.name = "bef";
            inPort.portColor = Color.green;
            inPort.portType = typeof(Node);
            inPort.allowMultiDrag = false;
            inputContainer.Add(inPort);
            /////////////////////////////////////
            var objPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            objPort.portName = "obj";
            objPort.name = "obj";
            objPort.portColor = Color.gray;
            objPort.portType = typeof(ObjectManager);
            objPort.allowMultiDrag = false;
            inputContainer.Add(objPort);
        }
        public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
        {
            OtherWorkNode temp = node as OtherWorkNode;
            var atemp = base.GenerateNodeAsset(temp, workAsset);
            ((OtherWorkNodeAsset)atemp).index = temp.index;
            atemp.canExe = true;
            return atemp;
        }
    }
}