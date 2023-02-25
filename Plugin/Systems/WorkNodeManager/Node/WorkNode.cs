using System.Collections;
using System.Collections.Generic;
using NodeEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//针对WorkAsset节点产生WorkNode
namespace NodeEditor
{
    public class WorkNode : BaseNode
    {
        public WorkAsset index;//保存的是对象
        public WorkNode(WorkAsset index)
        {
            style.backgroundColor = Color.blue;
            AssetType = typeof(WorkNodeAsset);
            this.index = index;
            this.title = index.name;
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

            var retVPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            retVPort.portName = "obj";
            retVPort.name = "obj";
            Color tc;
            ColorUtility.TryParseHtmlString("#282828", out tc);//Color.FromArgb(0x282828);//Color.cyan;
            tc *= 0.5f;
            retVPort.portColor = tc;
            retVPort.portType = typeof(object);
            outputContainer.Add(retVPort);

            if (index.breakNo != -1)
            {
                var expPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                expPort.portName = "break";
                expPort.name = "break";//发生预期外事件而终止
                expPort.portType = typeof(Node);
                expPort.portColor = Color.red;
                outputContainer.Add(expPort);
            }

            foreach (var val in index.dictvals)//输入节点值
            {
                var i = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
                //设置port显示的名称
                i.portName = val.name;
                i.name = val.name;

                Color tc1;
                ColorUtility.TryParseHtmlString("#282828", out tc1);//Color.FromArgb(0x282828);//Color.cyan;
                tc1 *= 0.5f;

                i.portColor = tc1;
                i.portType = val.type;
                inputContainer.Add(i);
            }

            ////////////////////////////////////
            var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            inPort.portName = "bef";
            inPort.name = "bef";
            inPort.portColor = Color.green;
            inPort.portType = typeof(Node);
            inPort.allowMultiDrag = false;
            inputContainer.Add(inPort);
            /////////////////////////////////////
        }
        public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
        {
            WorkNode temp = node as WorkNode;
            var atemp = base.GenerateNodeAsset(temp, workAsset);
            ((WorkNodeAsset)atemp).index = temp.index;
            atemp.canExe = true;
            return atemp;
        }
    }
}