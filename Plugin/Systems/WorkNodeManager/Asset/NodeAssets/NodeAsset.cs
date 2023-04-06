using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{

    //[CreateAssetMenu(fileName = "NewNodeAsset", menuName = "Assets/NodeAsset")]
    public class NodeAsset : SerializedScriptableObject
    {
        /// <summary>
        /// 标号
        /// </summary>
        public int no;
        //是否可执行
        public bool canExe;
        //数据类型
        public Type type;
        //节点位置
        public Rect pos;
        //输出
        public Dictionary<string, PortAsset> output;
        //输入值
        public Dictionary<string, PortAsset> inVal;
        /// <summary>
        /// 设置field
        /// </summary>
        public Dictionary<string, object> fields;
        public virtual Node retNode(int no)
        {
            //Debug.Log(2);
            BaseNode val = (BaseNode)Activator.CreateInstance(type);
            if (val.GetType() == typeof(DecisionNode) || val.GetType() == typeof(BehaviorNode))
            {
                val.LoadField(this);//输出节点
            }
            //得加一个根据某些东西来更新状态

            val.SetPosition(pos);
            val.name = no + "";
            return val;
        }
        public int NextNode(string port)//返回后一个节点
        {
            //Debug.Log(HashCode(port));
            if (output[port] == null)
            {
                //Debug.Log(HashCode(port));
            }
            return output[port].NodeNo;
        }
        public virtual string HashCode(string portName)
        {
            return null;
        }
    }
}