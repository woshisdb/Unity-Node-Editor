using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ObjectManagerAct;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    /// <summary>
    /// 进程的返回结果
    /// </summary>
    /// 
    public class RetVal
    {
        /// <summary>
        /// 当前进程的返回值
        /// </summary>
        public string retString = null;
        /// <summary>
        /// 当前进程的返回变量
        /// </summary>
        public object retVal = null;
        public RetVal()
        {

        }
        public RetVal(string retString, object retVal)
        {
            this.retString = retString;
            this.retVal = retVal;
        }
    }

    public class WorkProcessStack
    {
        public Stack<WorkProcess> exeProcess;//活动栈
        public RetVal retVal;
        public WorkProcessStack()
        {
            exeProcess = new Stack<WorkProcess>();
        }
        public WorkProcessStack(WorkProcess process)
        {
            retVal = new RetVal();
            exeProcess = new Stack<WorkProcess>();
            exeProcess.Push(process);
            process.retVal = retVal;
        }
        public WorkProcessStack(WorkAsset workAsset, ObjectManager objectManager)
        {
            retVal = new RetVal();
            exeProcess = new Stack<WorkProcess>();
            var x = new WorkProcess(workAsset, objectManager, this);
            exeProcess.Push(x);
            x.retVal = retVal;
        }
        public bool GetNowWork(out WorkProcess workProcess)
        {
            if (exeProcess.Count == 0)
            {
                workProcess = null;
                return false;
            }
            else
            {
                workProcess = exeProcess.Peek();
                return true;
            }
        }
        public WorkProcess GetNowWork()
        {
            return exeProcess.Peek();
        }
    }
    /// <summary>
    /// 每一个工作线程
    /// </summary>
    public class WorkProcess
    {
        public WorkProcessStack processStack;
        public ObjectManager objectManager;//当前主动交互的对象,
        public NodeAsset nowNode;//当前的节点
        public BaseAction nowAction;//当前对应的行为
        public WorkAsset workAsset;//互动的活动图
        public ActivityObject activityObject;
        public int no;//当前的标号no
                      //public string text="";//当前要显示的信息
        [ShowInInspector]
        public Dictionary<string, object> workEnvir;//一系列的工作期间访问的对象
        public Dictionary<string, object> NodeEnvir;//节点的访问环境
        public bool canExe;
        public bool stopStage;
        public RetVal retVal;
        public WorkProcess(ActivityObject activityObject, WorkProcessStack processStack)
        {
            this.processStack = processStack;
            this.activityObject = activityObject;//确定活动对象
            workAsset = activityObject.workAsset;
            objectManager = activityObject.objectManager;
            this.no = activityObject.workAsset.beginNo;//确定开始节点
            this.nowNode = activityObject.workAsset.nodes[no];
            workEnvir = new Dictionary<string, object>();
            NodeEnvir = new Dictionary<string, object>();
            canExe = true;
            stopStage = false;//当前阶段不暂停
                              //text = "";
        }
        public WorkProcess(WorkProcess workProcess, WorkProcessStack processStack)
        {
            this.processStack = processStack;
            this.activityObject = workProcess.activityObject;//确定活动对象
            this.workAsset = workProcess.workAsset;
            this.objectManager = workProcess.objectManager;
            this.no = workProcess.no;//确定开始节点
            this.nowNode = workProcess.workAsset.nodes[this.no];
            this.nowAction = workProcess.nowAction;
            workEnvir = workProcess.workEnvir;
            NodeEnvir = workProcess.NodeEnvir;
            canExe = workProcess.canExe;
            //text = "";
        }
        public WorkProcess(WorkAsset workAsset, ObjectManager objectManager, WorkProcessStack processStack)
        {
            this.processStack = processStack;
            ActivityObject activityObject = new ActivityObject(workAsset, objectManager);
            this.activityObject = activityObject;//确定活动对象
            this.workAsset = activityObject.workAsset;
            this.objectManager = activityObject.objectManager;
            this.no = activityObject.workAsset.beginNo;//确定开始节点
            this.nowNode = activityObject.workAsset.nodes[no];
            workEnvir = new Dictionary<string, object>();
            NodeEnvir = new Dictionary<string, object>();
            canExe = true;
            //text = "";
        }
        public bool GetVal<T>(string aimval, out T res)
        {
            object val;
            bool ret = nowNode.fields.TryGetValue(aimval, out val);
            res = (T)val;
            return ret;
        }
        public void SetEnvirmentVal(string name, object val)
        {
            workEnvir.Add(workAsset.nodes[workAsset.EnvirNo].HashCode(name), val);
        }
        public void CreateNewProcess(WorkAsset workAsset, List<Dictval> dictvals, List<object> objs, RetVal retVal)
        {
            WorkProcess workProcess = new WorkProcess(workAsset, objectManager, processStack);
            workProcess.retVal = retVal;
            processStack.exeProcess.Push(workProcess);
            for (int i = 0; i < dictvals.Count; i++)
            {
                workProcess.SetEnvirmentVal(dictvals[i].name, objs[i]);
            }
        }
        public void SetCallVal(string name, object obj)
        {
            workEnvir.Add("Call." + name, obj);
        }
        public object GetCallVal(string name)
        {
            return workEnvir["Call." + name];
        }
        public T GetCallVal<T>(string name)
        {
            return (T)workEnvir["Call." + name];
        }
    }
    /// <summary>
    /// 各种东西行为执行器
    /// </summary>
    public class ActivityExecute
    {
        public ObjectManager manager;//管理器
                                     //public WorkProcess GenerateProcess(ActivityObject activityObject)
                                     //{
                                     //    WorkProcess workProcess = new WorkProcess(activityObject);
                                     //    return workProcess;
                                     //}
        public WorkProcessStack GenerateProcessStack(ActivityObject activityObject)
        {
            WorkProcess workProcess = new WorkProcess(activityObject, null);
            WorkProcessStack workProcessStack = new WorkProcessStack(workProcess);
            workProcess.processStack = workProcessStack;
            return workProcessStack;
        }
        public WorkProcessStack GenerateProcessStack(WorkAsset workAsset, ObjectManager objectManager)
        {
            ActivityObject activityObject = new ActivityObject(workAsset, objectManager);
            WorkProcess workProcess = new WorkProcess(activityObject, null);
            WorkProcessStack workProcessStack = new WorkProcessStack(workProcess);
            workProcess.processStack = workProcessStack;
            return workProcessStack;
        }
        public ActivityExecute(ObjectManager manager)
        {
            this.manager = manager;
        }
        /// <summary>
        /// 工作进程是否结束
        /// </summary>
        /// <param name="workProcess"></param>
        public bool HasEnd(WorkProcessStack workProcess)
        {
            if (workProcess == null)
                return true;
            if (workProcess.exeProcess.Count == 0)//结束了
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据阶段来判断是否结束
        /// 结束返回true，暂停返回true
        /// </summary>
        /// <param name="workProcess"></param>
        public bool HasEndStage(WorkProcessStack workProcess)
        {
            if (workProcess == null)
                return true;
            if (workProcess.exeProcess.Count == 0)//结束了
            {
                return true;
            }
            else
            {
                WorkProcess now;
                workProcess.GetNowWork(out now);
                if (now.stopStage == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 给action赋值
        /// </summary>
        /// <param name="workProcess"></param>
        /// <param name="node"></param>
        public void InitProperty(WorkProcess workProcess, NodeAsset node)
        {
            if (node.fields.Count != 0)
                foreach (FieldInfo pi in workProcess.nowAction.GetType().GetFields())
                {
                    if (pi.GetCustomAttribute<ShowAttribute>() != null)
                        pi.SetValue(workProcess.nowAction, node.fields[pi.Name]);
                }
        }
        /// <summary>
        /// 根据节点的名字来初始化
        /// </summary>
        /// <param name="workProcess"></param>
        /// <param name="node"></param>
        public void InitNodeDic(WorkProcess workProcess, NodeAsset node)
        {
            workProcess.NodeEnvir.Clear();
            foreach (var i in workProcess.nowNode.inVal)//一系列的输入节点
            {
                if (i.Value != null)//值不为空
                {
                    object v = null;
                    bool jud = workProcess.workEnvir.TryGetValue(workProcess.workAsset.nodes[i.Value.NodeNo].HashCode(i.Value.PortName), out v);
                    if (jud == false)
                    {
                        v = null;
                    }
                    workProcess.NodeEnvir.Add(i.Key, v);
                }
                else//值为空
                {
                    workProcess.NodeEnvir.Add(i.Key, null);
                }
            }
            InitProperty(workProcess, node);
        }

        public void InitNodeDic(WorkProcess oldProcess, WorkProcess newProcess, NodeAsset node, bool type = false)
        {
            newProcess.workEnvir.Clear();
            foreach (var i in oldProcess.nowNode.inVal)//一系列的输入节点 
            {
                if (i.Value != null)
                {
                    object v;
                    var yes = oldProcess.workEnvir.TryGetValue(oldProcess.workAsset.nodes[i.Value.NodeNo].HashCode(i.Value.PortName), out v);
                    if (yes == false)
                    {
                        v = null;
                    }
                    if (type == true)//otherobj
                    {
                        if (i.Key.Split(":")[0] == "obj")//对象
                        {
                            newProcess.objectManager = (ObjectManager)v;
                        }
                        else
                        {
                            newProcess.workEnvir.Add(newProcess.workAsset.nodes[newProcess.workAsset.EnvirNo].HashCode(i.Key), v);
                        }
                    }
                    else
                    {
                        newProcess.workEnvir.Add(newProcess.workAsset.nodes[newProcess.workAsset.EnvirNo].HashCode(i.Key), v);
                    }
                }
            }
            oldProcess.workEnvir[node.HashCode("obj")] = newProcess.retVal.retVal;
        }

        /// <summary>
        /// 2：结束执行
        /// 1：正常执行
        /// 0：需要挂起
        /// </summary>
        /// <param name="workProcess"></param>
        /// <returns></returns>
        public void ExecuteNode(WorkProcessStack workProcess)
        {
            if (HasEnd(workProcess))//什么都不干
            {
            }
            else
            {
                WorkProcess work = workProcess.exeProcess.Peek();
                //Debug.Log(work.workAsset.name);
                if (work.workAsset.name == "选择桌子")
                {
                    //Debug.Log(1);
                }
                if (work.nowNode.canExe)//可执行节点
                {
                    if (work.nowNode.GetType() == typeof(DecisionNodeAsset))//要求别人去干的决策
                    {
                        //没有初始化
                        if (work.nowAction.hasInit == false)
                        {
                            work.nowAction.Begin(work);
                            work.nowAction.hasInit = true;
                        }
                        else
                        {
                            string ret;
                            if (work.nowAction.HaveEnd(work, out ret))//结束了
                            {
                                NextNode(workProcess, ret);
                            }
                            //else//没有结束
                            //{
                            //    work.nowAction.Execute(work);//执行
                            //}
                        }
                    }
                    else if (work.nowNode.GetType() == typeof(BehaviorNodeAsset))//自己的决策
                    {
                        if (work.nowAction.hasInit == false)
                        {
                            work.nowAction.Begin(manager, work);
                            work.nowAction.hasInit = true;
                        }
                        else
                        {
                            string ret;
                            if (work.nowAction.HaveEnd(manager, work, out ret))//结束了
                            {
                                NextNode(workProcess, ret);
                            }
                            //else//没有结束
                            //{
                            //    work.nowAction.Execute(manager, work);//执行
                            //}
                        }
                    }
                    else//WorkNodeAsset
                    {

                        NextNode(workProcess, workProcess.GetNowWork().workEnvir[workProcess.exeProcess.Peek().nowNode.HashCode("ret")] as string);//workNode或otherworkNode,,,,,,,,,,,,,,,,,,,,,,,,,,,,得改
                    }
                }
                else//不可执行，结束节点
                {
                    NextNode(workProcess, "succ");//转到下一个特殊节点，存在end节点
                }
            }
        }
        public static bool WorkAssetCanExecute(WorkAsset workAsset, ObjectManager mind1, ObjectManager mind2)
        {
            if (workAsset.judgeNo == -1)
            {
                return true;//可以执行
            }
            if (workAsset.nodes[workAsset.judgeNo].GetType() == typeof(DecisionNodeAsset))
            {
                var temp = workAsset.nodes[workAsset.judgeNo] as DecisionNodeAsset;
                var nowAction = Activator.CreateInstance(temp.port.DecisionType) as BaseAction;
                return nowAction.CanExecute(mind1);
            }
            else//BehaviorNodeAsset
            {
                var temp = workAsset.nodes[workAsset.judgeNo] as BehaviorNodeAsset;
                var nowAction = Activator.CreateInstance(temp.port.BehaviorType) as BaseAction;
                return nowAction.CanExecute(mind1, mind2);
            }
        }
        public static bool CanExecute(NodeAsset nodeAsset, ObjectManager mind1, ObjectManager mind2)
        {
            if (nodeAsset.GetType() == typeof(DecisionNodeAsset))
            {
                var temp = nodeAsset as DecisionNodeAsset;
                var nowAction = Activator.CreateInstance(temp.port.DecisionType) as BaseAction;
                return nowAction.CanExecute(mind1);
            }
            else//BehaviorNodeAsset
            {
                var temp = nodeAsset as BehaviorNodeAsset;
                var nowAction = Activator.CreateInstance(temp.port.BehaviorType) as BaseAction;
                return nowAction.CanExecute(mind1, mind2);
            }
        }
        /// <summary>
        /// 2：结束执行
        /// 1：正常执行
        /// 0：需要挂起
        /// </summary>
        /// <param name="workProcess"></param>
        /// <returns></returns>
        public void NextNode(WorkProcessStack workProcess, string ret)//下个节点
        {
            int next;
            WorkProcess work;
            if (workProcess.exeProcess.Count == 0)
            {
                return;
            }
            do
            {
                work = workProcess.exeProcess.Peek();
                //Debug.Log(work.workAsset);
                //Debug.Log(ret);
                next = work.workAsset.nextNode(work.nowNode, ret, work);//下一个节点的位置
                if (next == -1)//当前的活动结束了
                {
                    string restring = new string(work.retVal.retString);
                    object retval = work.retVal.retVal;
                    workProcess.exeProcess.Pop();
                    if (workProcess.exeProcess.Count > 0)
                    {
                        workProcess.exeProcess.Peek().workEnvir.Add(workProcess.exeProcess.Peek().nowNode.HashCode("ret"), restring);
                        //Debug.Log(workProcess.exeProcess.Peek().workAsset.name + ":"+workProcess.exeProcess.Peek().nowNode.HashCode("obj"));
                        if (retval != null)
                        {
                            string k = workProcess.exeProcess.Peek().nowNode.HashCode("obj");
                            //workProcess.exeProcess.Peek().workEnvir.Add(k, retval);
                            if (workProcess.exeProcess.Peek().workEnvir.ContainsKey(k))
                            {
                                workProcess.exeProcess.Peek().workEnvir[k] = retval;
                            }
                            else
                                workProcess.exeProcess.Peek().workEnvir.TryAdd(k, retval);
                        }
                        //var temp = workProcess.exeProcess.Peek().workEnvir;
                    }
                    else
                    {
                        //Debug.Log(1);
                        workProcess.retVal = new RetVal(restring, retval);
                    }
                    NextNode(workProcess, restring);//成功了
                    return;

                }
                else if (next == -2)//活动的中断
                {
                    workProcess.exeProcess.Pop();
                    NextNode(workProcess, "break");//中断了
                    return;
                }
                else//活动没结束
                {
                    if (work.workAsset.nodes[next].GetType() == typeof(BehaviorNodeAsset))//别人的行为
                    {
                        work.no = next;
                        BehaviorNodeAsset behaviorNodeAsset = (BehaviorNodeAsset)work.workAsset.nodes[next];
                        work.nowNode = behaviorNodeAsset;//对象名和映射的类名
                                                         //work.nowAction = ((ObjectManager)work.NodeEnvir["obj"]).behavious.FindBehavior(behaviorNodeAsset.port.BehaviorType.Name);//目标对象
                        work.nowAction = Activator.CreateInstance(behaviorNodeAsset.port.BehaviorType) as BaseAction;
                        InitNodeDic(work, work.nowNode);//当前节点
                        work.nowAction.Init((ObjectManager)work.NodeEnvir["obj"]);
                        //work.nowAction.Begin(manager, work);//开始执行
                        return;
                    }
                    else if (work.workAsset.nodes[next].GetType() == typeof(DecisionNodeAsset))//当前对象的行为
                    {
                        //Debug.Log( ((DecisionNodeAsset) work.workAsset.nodes[next]).port.DecisionName);
                        //Debug.Log(work.workAsset.name);
                        work.no = next;
                        DecisionNodeAsset decisionNodeAsset = (DecisionNodeAsset)work.workAsset.nodes[next];
                        work.nowNode = decisionNodeAsset;
                        //work.nowAction = work.objectManager.decisions.FindBehavior(decisionNodeAsset.port.DecisionName);
                        work.nowAction = Activator.CreateInstance(decisionNodeAsset.port.DecisionType) as BaseAction;
                        work.nowAction.Init(work.objectManager);
                        InitNodeDic(work, work.nowNode);//当前节点
                                                        //work.nowAction.Begin(work);//开始执行
                        return;
                    }
                    else if (work.workAsset.nodes[next].GetType() == typeof(OtherWorkNodeAsset))//....................
                    {
                        work.no = next;
                        OtherWorkNodeAsset workNodeAsset = (OtherWorkNodeAsset)work.workAsset.nodes[next];
                        work.nowNode = workNodeAsset;
                        WorkProcess process = new WorkProcess(new ActivityObject(workNodeAsset.index, work.objectManager), workProcess);
                        //process.retVal = new RetVal();
                        ((OtherWorkNodeAsset)work.workAsset.nodes[next]).call = new RetVal();
                        process.retVal = ((OtherWorkNodeAsset)work.workAsset.nodes[next]).call;
                        InitNodeDic(work, process, work.nowNode, true);//当前节点
                        workProcess.exeProcess.Push(process);
                        return;
                    }
                    else//WorkNodeAsset
                    {
                        work.no = next;
                        WorkNodeAsset workNodeAsset = (WorkNodeAsset)work.workAsset.nodes[next];
                        work.nowNode = workNodeAsset;
                        WorkProcess process = new WorkProcess(new ActivityObject(workNodeAsset.index, work.objectManager), workProcess);
                        ((WorkNodeAsset)work.workAsset.nodes[next]).call = new RetVal();
                        process.retVal = ((WorkNodeAsset)work.workAsset.nodes[next]).call;
                        InitNodeDic(work, process, work.nowNode);//当前节点
                        workProcess.exeProcess.Push(process);
                        return;
                    }
                }
            } while (true);
        }
    }
}