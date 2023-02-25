using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
//基础的行为节点
namespace NodeEditor
{
    public class BehaviorNode : ActionNode
    {
        public BehaviorPort behaviorPort;
        public BehaviorNode(BehaviorPort behaviorNodePort) : base(Color.yellow, typeof(BehaviorNodeAsset), behaviorNodePort.ObjectName + ":" + behaviorNodePort.BehaviorType.Name, Activator.CreateInstance(behaviorNodePort.BehaviorType) as BaseAction)
        {
            this.behaviorPort = behaviorNodePort;
        }

        public override void ReGeneratePort(List<string> Out, List<Dictval> InVal, List<Dictval> OutVal)
        {
            outputContainer.Clear();
            inputContainer.Clear();
            var befPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            befPort.portName = "bef";
            befPort.name = "bef";
            befPort.portColor = Color.green;
            befPort.portType = typeof(Node);
            inputContainer.Add(befPort);

            var objPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            objPort.portName = "obj";
            objPort.name = "obj";
            //objPort.portColor = Color.cyan;
            Color x;
            ColorUtility.TryParseHtmlString("#282828", out x);//Color.FromArgb(0x282828);//Color.cyan;
            objPort.portColor = x;
            objPort.portType = typeof(ObjectManager);
            inputContainer.Add(objPort);

            base.ReGeneratePort(Out, InVal, OutVal);
        }

        //public void UpdatePort()
        //{
        //    //为nowAction赋值
        //    foreach (var x in contentContainer.Children())
        //    {
        //        if (x.GetType() == typeof(TextField))
        //        {
        //            TextField field = (TextField)x;
        //            object val;
        //            val = field.value;
        //            nowAction.GetType().GetProperty(field.label).SetValue(nowAction, val);
        //        }
        //        else if (x.GetType() == typeof(IntegerField))
        //        {
        //            IntegerField field = (IntegerField)x;
        //            object val;
        //            val = field.value;
        //            nowAction.GetType().GetProperty(field.label).SetValue(nowAction, val);
        //        }
        //        else if (x.GetType() == typeof(FloatField))
        //        {
        //            FloatField field = (FloatField)x;
        //            object val;
        //            val = field.value;
        //            nowAction.GetType().GetProperty(field.label).SetValue(nowAction, val);
        //        }
        //        else if (x.GetType() == typeof(Toggle))
        //        {
        //            Toggle field = (Toggle)x;
        //            object val;
        //            val = field.value;
        //            nowAction.GetType().GetProperty(field.label).SetValue(nowAction, val);
        //        }
        //        else if (x.GetType() == typeof(EnumField))
        //        {
        //            EnumField field = (EnumField)x;
        //            object val;
        //            val = field.value;
        //            nowAction.GetType().GetProperty(field.label).SetValue(nowAction, val);
        //        }
        //    }
        //    //根据nowAction的输出结果来重新生成节点
        //    var res = nowAction.UpdateOutPort();
        //    if (res != null && (res.K1 != null || res.K2 != null))
        //    {
        //        if (res.K1 == null)
        //        {
        //            res.K1 = nowAction.Out;
        //        }
        //        if (res.K2 == null)
        //        {
        //            res.K2 = nowAction.OutVal;
        //        }
        //        ReGeneratePort(res.K1, nowAction.InVal, res.K2);
        //    }
        //}
        public override NodeAsset GenerateNodeAsset(Node node, WorkAsset workAsset)//最基本的初始化
        {
            var temp = base.GenerateNodeAsset(node, workAsset);
            temp.canExe = true;
            ((BehaviorNodeAsset)temp).port = ((BehaviorNode)node).behaviorPort;
            return temp;
        }
    }
}