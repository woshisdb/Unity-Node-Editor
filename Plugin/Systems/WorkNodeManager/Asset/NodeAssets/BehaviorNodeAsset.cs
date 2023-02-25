using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// 保存节点的类型
    /// </summary>
    public class BehaviorPort
    {
        public string ObjectName;//行为的名字
        public Type BehaviorType;
        public BehaviorPort(string objectName, Type behaviorType)
        {
            ObjectName = objectName;
            BehaviorType = behaviorType;
        }
        public BehaviorPort()
        {

        }
    }
    public class BehaviorNodeAsset : NodeAsset
    {
        public BehaviorPort port;
        public override Node retNode(int no)
        {
            BaseNode val = (BaseNode)Activator.CreateInstance(typeof(BehaviorNode), port);
            val.SetPosition(pos);
            val.LoadField(this);
            val.name = no + "";
            canExe = true;
            return val;
        }
        public override string ToString()
        {
            return "BehaviorNode:\n" + port.ObjectName + "/" + port.BehaviorType.Name;
        }
        public override string HashCode(string portName)
        {
            return no + ":" + portName;
            //return no + ":" + "BehaviorNode:" +port.ObjectName + ":" + port.BehaviorType.Name+":"+portName;
        }
    }
}