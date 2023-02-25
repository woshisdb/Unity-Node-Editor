using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    public class EndNodeAsset : NodeAsset
    {
        /// <summary>
        /// 结束的资源
        /// </summary>
        public List<string> ret;
        public override string ToString()
        {
            return "EndNode";
        }
        public override string HashCode(string portName)
        {
            return no + ":" + "EndNode:" + portName;
        }
        public override Node retNode(int no)
        {
            var x = base.retNode(no);
            if (ret == null)
                ret = new List<string>();
            if (ret.Count == 0)
            {
                ret.Add("succ");
            }
            ((EndNode)x).SetRetVal(ret);
            ((EndNode)x).UpdateEndPort();
            return x;
        }
    }
}