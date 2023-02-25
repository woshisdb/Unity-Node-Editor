using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    public class BreakNodeAsset : NodeAsset
    {
        public override string ToString()
        {
            return "BreakNode";
        }
        public override string HashCode(string portName)
        {
            return "BreakNode:" + portName;
        }
    }
}