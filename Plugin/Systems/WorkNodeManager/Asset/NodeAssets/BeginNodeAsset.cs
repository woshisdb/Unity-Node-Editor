using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeEditor
{
    public class BeginNodeAsset : NodeAsset
    {
        public override string ToString()
        {
            return "BeginNode";
        }
        public override string HashCode(string portName)
        {
            return "BeginNode:" + portName;
        }
    }
}