using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    public class DecisionPort
    {
        public string DecisionName;//行为的名字
        public Type DecisionType;
        public DecisionPort(string decisionName, Type decisionType)
        {
            DecisionName = decisionName;
            DecisionType = decisionType;
        }
        public DecisionPort()
        {

        }
    }
    public class DecisionNodeAsset : NodeAsset
    {
        public DecisionPort port;
        public override Node retNode(int no)
        {
            BaseNode val = (BaseNode)Activator.CreateInstance(typeof(DecisionNode), port);
            val.SetPosition(pos);
            //Debug.Log(port.DecisionName);
            val.LoadField(this);
            val.name = no + "";
            canExe = true;
            return val;
        }
        public override string ToString()
        {
            return "DecisionNode:\n" + port.DecisionName;
        }
        public override string HashCode(string portName)
        {
            return no + ":" + portName;
            //return no + ":" + "DecisionNode:" +port.DecisionName + ":" + portName;
        }
    }
}