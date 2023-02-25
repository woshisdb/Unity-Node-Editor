using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditor
{
    public class ListElement : BaseElement
    {
        VisualElement front;
        VisualElement content;
        Type contentType;//当前类型
        public ListElement(Type type, string Name, BaseNode baseNode) : base(type, Name, baseNode)
        {
            name = Name;
            front = new VisualElement();
            front.name = "front";
            contentType = type.GetGenericArguments()[0];
            Label label = new Label(Name);
            front.Add(label);
            Button button = new Button();
            button.name = "Add";
            button.text = "Add";
            button.clicked += (() => AddElement());
            front.Add(button);
            Add(front);
            //////////////////////////////////////////
            content = new VisualElement();
            Add(content);
        }
        bool NeedDfs(Type contentType)
        {
            bool needDfs = false;
            foreach (var x in contentType.GetFields())
            {
                if (x.GetCustomAttribute<ShowAttribute>() != null)
                {
                    needDfs = true;
                }
            }
            return needDfs;
        }
        public VisualElement AddElement()
        {
            var val = new VisualElement();
            val.style.backgroundColor = Color.black;
            Button button = new Button();
            button.text = "remove";
            button.clicked += (() => { content.Remove(val); });
            val.Add(button);

            if (NeedDfs(contentType) == false)//不用迭代
            {
                var x = ElementFactory.GetElement(contentType, "val", baseNode);
                val.Add(x);
            }
            else//表示需要迭代
            {
                foreach (var pi in contentType.GetFields())//获取一系列property
                {
                    if (pi.GetCustomAttribute<ShowAttribute>() != null)
                    {
                        var x = ElementFactory.GetElement(pi.FieldType, pi.Name, baseNode);
                        val.Add(x);
                    }
                }
            }
            content.Add(val);
            baseNode.RefreshExpandedState();
            return val;
        }

        public void ClearElement()
        {
            content.Clear();
        }

        public override object GetVal()
        {
            dynamic res = Activator.CreateInstance(type);
            foreach (var x in content.Children())
            {
                dynamic ele = null;
                if (NeedDfs(contentType) == false)
                {
                    ele = x.Q<BaseElement>("val").GetVal();
                }
                else
                {
                    ele = Activator.CreateInstance(contentType);//创建一个type对象
                    foreach (var pi in contentType.GetFields())//获取一系列property
                    {
                        if (pi.GetCustomAttribute<ShowAttribute>() != null)
                        {
                            var y = x.Q<BaseElement>(pi.Name);
                            pi.SetValue(ele, y.GetVal());
                        }
                    }
                }
                res.Add(ele);
            }
            return res;
        }

        public override void SetVal(object val)
        {
            ClearElement();
            dynamic res = val;
            foreach (var x in res)
            {
                var y = AddElement();
                if (NeedDfs(contentType) == false)
                {
                    y.Q<BaseElement>("val").SetVal(x);
                }
                else
                {
                    foreach (var pi in contentType.GetFields())//获取一系列property
                    {
                        if (pi.GetCustomAttribute<ShowAttribute>() != null)
                        {
                            object temp;
                            temp = pi.GetValue(x);
                            y.Q<BaseElement>(pi.Name).SetVal(temp);
                        }
                    }
                }
            }
        }
    }
}