using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{
    public class EndNode : BaseNode
    {
        ///// <summary>
        ///// 返回节点
        ///// </summary>
        //public List<string> ret;
        BaseElement list;
        public EndNode()
        {
            style.backgroundColor = Color.red;
            AssetType = typeof(EndNodeAsset);
            //nodeType = NodeType.endNode;
            title = "end";
            Button UpdateButton = new Button();
            UpdateButton.text = "update";
            UpdateButton.RegisterCallback<ClickEvent>(ev => UpdateEndPort());
            contentContainer.Add(UpdateButton);
            list = ElementFactory.GetElement(typeof(List<string>), "return", this);
            contentContainer.Add(list);
            SetRetVal(new List<string>() { "succ" });
            ReGeneratePort();
        }
        public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
        {
            var temp = base.GenerateNodeAsset(node, workAsset);
            ((EndNodeAsset)temp).ret = list.GetVal() as List<string>;
            temp.canExe = false;
            return temp;
        }
        public void SetRetVal(List<string> strings)
        {
            list.SetVal(strings);
        }
        public virtual void UpdateEndPort()
        {
            //list = ElementFactory.GetElement(typeof( List<string>), "return", this);
            //contentContainer.Add(list);
            ReGeneratePort();
        }
        public void ReGeneratePort()
        {
            List<string> ret = list.GetVal() as List<string>;
            if (ret.Count == 0)
            {
                ret = new List<string>() { "succ" };
                list.SetVal(ret);
            }
            outputContainer.Clear();
            inputContainer.Clear();
            if (ret == null || ret.Count == 0)
            {
                ret = new List<string>();
                var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
                inPort.portName = "succ";
                inPort.name = "succ";
                inPort.portColor = Color.green;
                inPort.portType = typeof(Node);
                inPort.allowMultiDrag = false;
                inputContainer.Add(inPort);
            }
            else
            {
                foreach (var val in ret)//输入节点值
                {
                    var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
                    //设置port显示的名称
                    inPort.portName = val;
                    inPort.name = val;
                    inPort.portColor = Color.green;
                    inPort.portType = typeof(Node);
                    inPort.allowMultiDrag = false;
                    inputContainer.Add(inPort);
                }
            }
            //输入值
            var inPortV = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            inPortV.portName = "obj";
            inPortV.name = "obj";
            inPortV.portColor = Color.cyan;
            inPortV.portType = typeof(object);
            inPortV.allowMultiDrag = false;
            inputContainer.Add(inPortV);
            RefreshExpandedState();
        }
    }
}