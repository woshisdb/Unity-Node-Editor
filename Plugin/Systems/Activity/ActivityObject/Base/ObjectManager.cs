using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// ��������Ҫ�̳����
    /// </summary>
    public class ObjectManager : SerializedMonoBehaviour
    {
        private bool FilterWork(WorkAsset workAsset)
        {
            if (objectStruct.workAssetDict.Contains(workAsset))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HideInInspector]
        public ActivityExecute activityExecute;
        /// <summary>
        /// ����ĳ������ṹ
        /// </summary>
        public ObjectStruct objectStruct;
        /// <summary>
        /// ִ�н�������true��û����Ϊfalse
        /// </summary>
        /// <param name="nowProcess"></param>
        /// <returns></returns>
        public bool ExeToFinish(WorkProcessStack nowProcess)
        {
            if (activityExecute.HasEndStage(nowProcess) == false)//���ж�������
            {
                activityExecute.ExecuteNode(nowProcess);
            }
            return activityExecute.HasEnd(nowProcess);
        }

        /****************��Ϊ�Ļغ�*******************/
        /// <summary>
        /// ��ʼ����Ϊ
        /// </summary>
        [Button("InitBehavior")]
        public virtual void Init()
        {
            activityExecute = new ActivityExecute(this);
        }
    }
}