using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    public class JudgeNodeAsset : NodeAsset
    {
        public override string ToString()
        {
            return "JudgeNode";
        }
        public override string HashCode(string portName)
        {
            return "JudgeNode:" + portName;
        }
    }
}