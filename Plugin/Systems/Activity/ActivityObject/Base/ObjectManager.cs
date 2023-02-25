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
    /// 交互对象都要继承这个
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
        /// 对象的持续化结构
        /// </summary>
        public ObjectStruct objectStruct;
        /// <summary>
        /// 执行结束返回true，没结束为false
        /// </summary>
        /// <param name="nowProcess"></param>
        /// <returns></returns>
        public bool ExeToFinish(WorkProcessStack nowProcess)
        {
            if (activityExecute.HasEndStage(nowProcess) == false)//不中断则运行
            {
                activityExecute.ExecuteNode(nowProcess);
            }
            return activityExecute.HasEnd(nowProcess);
        }

        /****************行为的回合*******************/
        /// <summary>
        /// 初始化行为
        /// </summary>
        [Button("InitBehavior")]
        public virtual void Init()
        {
            activityExecute = new ActivityExecute(this);
        }
    }
}