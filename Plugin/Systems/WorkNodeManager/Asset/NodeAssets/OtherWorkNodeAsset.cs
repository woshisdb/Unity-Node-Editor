using System.Collections;
using System.Collections.Generic;
using NodeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{

    public class OtherWorkNodeAsset : NodeAsset
    {
        public WorkAsset index;//一个活动
        public RetVal call;
        public override Node retNode(int no)
        {
            Node val = new OtherWorkNode(index);
            val.SetPosition(pos);
            val.name = no + "";
            return val;
        }
        public override string ToString()
        {
            return "OtherWorkNode:\n" + index.name;
        }
        public override string HashCode(string portName)
        {
            return no + ":" + portName;
            //return no + ":" + "OtherWorkNode:" + index.name + ":" + portName;
        }
    }
}