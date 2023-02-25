using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NodeEditor
{
    /// <summary>
    /// ��ĸ���
    /// </summary>
    public class MindBehaveManager
    {
        public Dictionary<string, BaseAction> behaviors;
        /// <summary>
        /// ������Ϊ������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseAction FindBehavior(string type)
        {
            return behaviors[type];
        }
        /// <summary>
        /// ������������Ϊ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseAction FindBehavior(Type type)
        {
            return behaviors[type.Name];
        }
        /// <summary>
        /// ��ʼ������,ͨ����ʼ������һϵ�е���Ϊ�����
        /// </summary>
        /// <param name="mind"></param>
        public MindBehaveManager()
        {
            behaviors = new Dictionary<string, BaseAction>();
        }
        public void AddBehavior(BaseAction baseAction)
        {
            behaviors.Add(baseAction.GetType().Name, baseAction);
        }
        public void ConstructAction(List<Type> lists, ObjectManager objectManager)
        {
            behaviors.Clear();
            for (int i = 0; i < lists.Count; i++)
            {
                behaviors.Add(lists[i].Name, (BaseAction)Activator.CreateInstance(lists[i]));
            }
            foreach (var v in behaviors)
            {
                v.Value.Init(objectManager);
            }
        }
    }
}