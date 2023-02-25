using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

/// <summary>
/// 一个活动,是将程序映射到具体对象的一个映射关系,允许继承等操作
/// </summary>
namespace NodeEditor
{
    public class ActivityObject
    {
        public WorkAsset workAsset;//一个工作,节点图
        public ObjectManager objectManager;//对应的对象
        public ActivityObject(WorkAsset workAsset, ObjectStruct objectStruct, ObjectManager objectManager)
        {
            this.workAsset = workAsset;
            this.objectManager = objectManager;
        }
        public ActivityObject()
        {
            workAsset = null;
            objectManager = null;
        }
        public ActivityObject(WorkAsset workAsset, ObjectManager objectManager)
        {
            this.workAsset = workAsset;
            this.objectManager = objectManager;
        }
    }
}