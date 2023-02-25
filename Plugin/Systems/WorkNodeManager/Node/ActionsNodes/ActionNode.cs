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
    public class ActionNode : BaseNode
    {
        public ActionNode(Color color, Type assetType, string title, BaseAction x)
        {
            style.backgroundColor = color;
            AssetType = assetType;
            //名字
            this.title = title;
            nowAction = x;
            Button UpdateButton = new Button();
            UpdateButton.text = "update";
            UpdateButton.RegisterCallback<ClickEvent>(ev => UpdatePort());
            contentContainer.Add(UpdateButton);
            SetField(x);
            ReGeneratePort(x.Out, x.InVal, x.OutVal);
        }
        public virtual void UpdatePort()
        {
            foreach (var x in nowAction.GetType().GetFields())
            {
                if (x.GetCustomAttribute<ShowAttribute>() != null)
                    x.SetValue(nowAction, contentContainer.Q<BaseElement>(x.Name).GetVal());
            }
            //根据nowAction的输出结果来重新生成节点
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
                nowAction.InVal = res.K3;
                nowAction.OutVal = res.K2;
                nowAction.Out = res.K1;
                ReGeneratePort(res.K1, res.K3, res.K2);
            }
        }
        public override void ReGeneratePort(List<string> Out, List<Dictval> InVal, List<Dictval> OutVal)
        {
            foreach (string val in Out)
            {
                var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
                //设置port显示的名称
                outPort.portName = val;
                outPort.name = val;
                outPort.portType = typeof(Node);
                outPort.portColor = Color.red;
                outputContainer.Add(outPort);
            }
            foreach (var val in InVal)//输入节点值
            {
                var inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
                //设置port显示的名称
                inPort.portName = val.name;
                inPort.name = val.name;
                Color x;
                ColorUtility.TryParseHtmlString("#282828", out x);//Color.FromArgb(0x282828);//Color.cyan;
                x *= 0.5f;
                inPort.portColor = x;
                //inPort.portColor = Color.cyan;
                inPort.portType = val.type;
                inputContainer.Add(inPort);
            }
            foreach (var val in OutVal)//输出节点值
            {
                var outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
                //设置port显示的名称
                outPort.portName = val.name;
                outPort.name = val.name;
                outPort.portType = val.type;
                Color x;
                ColorUtility.TryParseHtmlString("#282828", out x);//Color.FromArgb(0x282828);//Color.cyan;
                x *= 0.5f;
                outPort.portColor = x;
                //outPort.portColor = Color.blue;
                outputContainer.Add(outPort);
            }
            RefreshExpandedState();
        }
    }
}