using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    public struct Dictval
    {
        public string name;
        public Type type;
        public Dictval(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class Pair<U, V>
    {
        /// <summary>
        /// out
        /// </summary>
        public U K1;
        /// <summary>
        /// outval
        /// </summary>
        public V K2;
        /// <summary>
        /// inval
        /// </summary>
        public V K3;
        public Pair()
        {
            K1 = default(U);
            K2 = default(V);
            K3 = default(V);
        }
        public Pair(U k1, V k2)
        {
            K1 = k1;
            K2 = k2;
            K3 = default(V);
        }
        public Pair(U k1, V k2, V k3) : this(k1, k2)
        {
            K3 = k3;
        }
    }
    public abstract class BaseAction
    {
        /// <summary>
        /// 输出节点
        /// </summary>
        public List<string> Out = new List<string>();
        public List<Dictval> InVal = new List<Dictval>();
        public List<Dictval> OutVal = new List<Dictval>();
        public int wastertime = 0;//花费的时间
        public bool hasInit;

        public virtual void Begin(ObjectManager mind, WorkProcess workProcess)
        {

        }

        public virtual void Execute(ObjectManager mind, WorkProcess workProcess)
        {

        }


        public virtual bool HaveEnd(ObjectManager mind, WorkProcess workProcess, out string result)
        {
            result = null;
            return true;
        }

        public virtual string ShowDetail(ObjectManager mind, WorkProcess workProcess)
        {
            return null;
        }

        public virtual string ShowDetail(WorkProcess workProcess)
        {
            return null;
        }
        public virtual bool HaveEnd(WorkProcess workProcess, out string result)
        {
            result = null;
            return true;
        }
        /// <summary>
        /// 执行当前行为的方法,持续调用
        /// </summary>
        /// <returns></returns>
        public virtual void Execute(WorkProcess workProcess)
        {

        }
        public virtual void Begin(WorkProcess workProcess)//当前活动的工作环境
        {

        }
        /// <summary>
        /// 根据当前的节点信息来更新节点的输入输出节点，和当前的状态
        /// </summary>
        /// <returns></returns>
        public virtual Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            return null;
        }
        public object GetInputVal(WorkProcess workProcess, string u)
        {
            object val;
            val = workProcess.NodeEnvir[u];
            return val;
        }
        /// <summary>
        /// 获取数据,从NodeEnvir中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public T GetInputVal<T>(WorkProcess workProcess, string u)
        {
            //Debug.Log(GetType().Name);
            T val;
            val = (T)workProcess.NodeEnvir[u];
            return val;
        }


        /// <summary>
        /// 设置数据,从NodeEnvir中
        /// </summary>
        /// <param name="workProcess"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public void SetInputVal(WorkProcess workProcess, string u, object v)
        {
            workProcess.NodeEnvir[u] = v;
        }

        /// <summary>
        /// 设置自己的数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public void setDictVal(WorkProcess workProcess, string u, object v)
        {
            //Debug.Log(u);
            u = workProcess.nowNode.HashCode(u);//字符的名字
            if (workProcess.workEnvir.TryAdd(u, v) == false)
            {
                workProcess.workEnvir[u] = v;
            }
        }
        public bool RemoveDictVal(WorkProcess workProcess, string u)
        {
            u = workProcess.nowNode.HashCode(u);
            return workProcess.workEnvir.Remove(u);
        }

        /// <summary>
        /// 判断自己有没有设置数值
        /// </summary>
        /// <param name="workProcess"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool getDictVal(WorkProcess workProcess, string u, out object Obj)
        {
            u = workProcess.nowNode.HashCode(u);//字符的名字
            return workProcess.workEnvir.TryGetValue(u, out Obj);
        }
        public bool getDictVal<T>(WorkProcess workProcess, string u, out T Obj)
        {
            u = workProcess.nowNode.HashCode(u);//字符的名字
            object temp;
            bool val = workProcess.workEnvir.TryGetValue(u, out temp);
            if (val == true)
            {
                Obj = (T)temp;
            }
            else
            {
                Obj = default(T);
            }
            return val;
        }
        public T getDictVal<T>(WorkProcess workProcess, string u)
        {
            u = workProcess.nowNode.HashCode(u);//字符的名字
            return (T)workProcess.workEnvir[u];
        }
        /// <summary>
        /// 判断自己有没有设置数值
        /// </summary>
        /// <param name="workProcess"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool hasDicVal(WorkProcess workProcess, string u)
        {
            u = workProcess.nowNode.HashCode(u);//字符的名字
            return workProcess.workEnvir.ContainsKey(u);//包含对象
        }
        public BaseAction()//初始化来判断节点的类型
        {
            hasInit = false;
            Out.Add("succ");
        }
        public abstract void Init(ObjectManager objectManager);//初始化节点的信息
        /// <summary>
        /// 是否可以执行这个行为,用于envirment的判断
        /// </summary>
        /// <param name="workProcess"></param>
        /// <returns></returns>
        public virtual bool CanExecute(ObjectManager mind)
        {
            return true;
        }
        public virtual bool CanExecute(ObjectManager mind1, ObjectManager mind2)
        {
            return true;
        }

        //public object GetEnvirment(string val,WorkProcess workProcess)
        //{
        //    return workProcess.workAsset.envirVal[val];
        //}
    }
    public abstract class BaseAction<V> : BaseAction where V : ObjectManager
    {
        /// <summary>
        /// 当前的行为是否需要记住后面行为的收益
        /// </summary>
        public bool shouldMemory = false;//
        /// <summary>
        /// 当前附属的对象..一定是固定的
        /// </summary>
        public V objectManager;
        public override void Init(ObjectManager objectManager)//初始化节点的信息
        {
            this.objectManager = objectManager as V;
            this.Out = null;
            this.InVal = null;
            this.OutVal = null;
        }
    }
}