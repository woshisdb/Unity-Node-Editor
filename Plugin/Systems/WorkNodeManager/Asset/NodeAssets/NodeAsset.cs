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
        /// ���
        /// </summary>
        public int no;
        //�Ƿ��ִ��
        public bool canExe;
        //��������
        public Type type;
        //�ڵ�λ��
        public Rect pos;
        //���
        public Dictionary<string, PortAsset> output;
        //����ֵ
        public Dictionary<string, PortAsset> inVal;
        /// <summary>
        /// ����field
        /// </summary>
        public Dictionary<string, object> fields;
        public virtual Node retNode(int no)
        {
            //Debug.Log(2);
            BaseNode val = (BaseNode)Activator.CreateInstance(type);
            if (val.GetType() == typeof(DecisionNode) || val.GetType() == typeof(BehaviorNode))
            {
                val.LoadField(this);//����ڵ�
            }
            //�ü�һ������ĳЩ����������״̬

            val.SetPosition(pos);
            val.name = no + "";
            return val;
        }
        public int NextNode(string port)//���غ�һ���ڵ�
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