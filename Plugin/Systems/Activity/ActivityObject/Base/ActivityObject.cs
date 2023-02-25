using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

/// <summary>
/// һ���,�ǽ�����ӳ�䵽��������һ��ӳ���ϵ,����̳еȲ���
/// </summary>
namespace NodeEditor
{
    public class ActivityObject
    {
        public WorkAsset workAsset;//һ������,�ڵ�ͼ
        public ObjectManager objectManager;//��Ӧ�Ķ���
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