using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NodeEditor
{
    /// <summary>
    /// 活动的更新
    /// </summary>
    public class MindBehaveManager
    {
        public Dictionary<string, BaseAction> behaviors;
        /// <summary>
        /// 根据行为类型找
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseAction FindBehavior(string type)
        {
            return behaviors[type];
        }
        /// <summary>
        /// 根据类型找行为
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseAction FindBehavior(Type type)
        {
            return behaviors[type.Name];
        }
        /// <summary>
        /// 初始化方法,通过初始化构造一系列的行为或决定
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