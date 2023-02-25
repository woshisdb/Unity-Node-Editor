using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeEditor;
using UnityEngine;


namespace ObjectManagerAct
{
    /// <summary>
    /// 一系列环境变量的设置类
    /// </summary>
    [Id("ObjectManager")]
    public class Envirment : Decision<ObjectManager>
    {
        [Show]
        public string variables ;
        public Envirment()
        {
            Out.Clear();
            OutVal.Clear();
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            result = "succ";
            return true;
        }
        /// <summary>
        /// val:int/
        /// </summary>
        /// <returns></returns>
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            Pair<List<string>, List<Dictval>> res = new Pair<List<string>, List<Dictval>>();
            res.K1 = null;
            res.K2 = new List<Dictval>();
            var temp = variables.Split("/").ToList();
            if(temp.Count > 0 && temp[0]=="")
            {
                temp.RemoveAt(0);
            }
            foreach (var part in temp)
            {
                var dic = part.Split(":").ToList();
                //Debug.Log(BasicFunction.GetTypeByName(dic[1]));
                res.K2.Add(new Dictval(dic[0], BasicFunction.GetTypeByName(dic[1])));
            }
            return res;
        }
    }
    [Id("ObjectManager")]
    public class DebugString : Decision<ObjectManager>
    {
        [Show]
        public string variables ;
        public DebugString()
        {
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            Debug.Log(workProcess.workAsset.name);
            Debug.Log(variables);
            result = "succ";
            return true;
        }
    }

    /// <summary>
    /// 输出字符串
    /// </summary>
    [Id("ObjectManager")]
    public class OutString : Decision<ObjectManager>
    {
        [Show]
        public string val ;
        public OutString()
        {
            OutVal.Add(new Dictval("res", typeof(string)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            setDictVal(workProcess, "res", val);
            result = "succ";
            return true;
        }
    }

    [Id("ObjectManager")]
    public class Test : Decision<ObjectManager>
    {
        [Show]
        public List<string> x;
        public Test()
        {
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            result = "succ";
            return true;
        }
    }


    [Id("ObjectManager")]
    public class SelectString : Decision<ObjectManager>
    {
        [Show]
        public string val;
        public SelectString()
        {
            InVal.Add(new Dictval("val", typeof(string)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            var x=GetInputVal<string>(workProcess, "val");
            var temp = val.Split('/');
            for(int i=0;i<temp.Count();i++)
            {
                if(temp[i].Equals(x))
                {
                    result=temp[i];
                    return true;
                }
            }
            result = "*";
            return true;
        }
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            var res=new Pair<List<string>, List<Dictval>>();
            res.K3 = new List<Dictval>();
            res.K3.Add(new Dictval("val",typeof(string)));
            res.K1 =val.Split('/').ToList();
            res.K1.Add("*");
            return res;
        }
    }


    [Id("ObjectManager")]
    public class ChoseSelect : Decision<ObjectManager>
    {
        [Show]
        public string outports ;
        public ChoseSelect()
        {
            Out.Clear();
            Out.Add("succ");
            OutVal.Clear();
            InVal.Clear();
            InVal.Add(new Dictval("Option",typeof(string)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            result = GetInputVal<string>(workProcess, "Option");
            return true;
        }
        public List<string> resPort()
        {
            return outports.Split("/").ToList();
        }
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            var res=new Pair<List<string>, List<Dictval>>();
            res.K1=resPort();
            res.K2 = null;
            return res;
        }
    }

    [Id("ObjectManager")]
    public class WaitTime : Decision<ObjectManager>
    {
        [Show]
        public int Time ;
        public WaitTime()
        {
        }
        private int nowtime=0;
        public override void Begin(WorkProcess workProcess)
        {
            nowtime = 0;
        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            if(nowtime<Time)
            {
                nowtime++;
                result = null;
                return false;
            }
            else
            {
                result = "succ";
                return true;
            }
        }
    }


    [Id("ObjectManager")]
    public class ExeWorkAsset : Decision<ObjectManager>
    {
        [Show]
        public WorkAsset workAssetTp ;
        /// <summary>
        /// 结束执行
        /// </summary>
        bool end;
        RetVal retVal;
        public ExeWorkAsset()
        {
            InVal.Add(new Dictval("workAsset",typeof(WorkAsset)));
        }
        public override void Begin(WorkProcess workProcess)
        {
            end = false;
        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            if(end==false)
            {
                var workAsset=GetInputVal<WorkAsset>(workProcess, "workAsset");
                //Debug.Log(workAsset.name+":");
                end = true;
                List<object> list = new List<object>();
                foreach (var x in workAssetTp.dictvals)
                {
                    list.Add(GetInputVal(workProcess,x.name));
                }
                retVal=new RetVal();
                workProcess.CreateNewProcess(workAsset, workAssetTp.dictvals, list,retVal);
                result = null;
                return false;
            }
            result = retVal.retString;
            setDictVal(workProcess,"obj",retVal.retVal);
            return true;
        }
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            Pair<List<string>,List<Dictval>> pair = new Pair<List<string>, List<Dictval>>();
            pair.K1 = new List<string>();
            pair.K2 = new List<Dictval>() { new Dictval("obj",typeof(object)) };
            foreach(var x in ((EndNodeAsset)( workAssetTp.nodes[workAssetTp.endNo])).ret)
            {
                pair.K1.Add(x);
            }
            pair.K3 = new List<Dictval>();
            pair.K3.Add(new Dictval("workAsset", typeof(WorkAsset)));
            if (workAssetTp != null)
                foreach (var x in workAssetTp.dictvals)
                    pair.K3.Add(new Dictval(x.name, x.type));
            else
                pair.K3 = null;
            return pair;
        }
    }
    //[Id("ObjectManager")]
    //public class GetEnvirVal : Decision<ObjectManager>
    //{
    //    public GetEnvirVal()
    //    {
    //        InVal.Add(new Dictval("name",typeof(object)));
    //        OutVal.Add(new Dictval("val",typeof(object)));
    //    }
    //    public override void Begin(WorkProcess workProcess)
    //    {

    //    }
    //    public override void Execute(WorkProcess workProcess)
    //    {
    //    }
    //    public override bool HaveEnd(WorkProcess workProcess, out string result)
    //    {
    //        var x=workProcess.workAsset.envirVal[GetInputVal<string>(workProcess, "name")];
    //        setDictVal(workProcess, "val", x);
    //        result = "succ";
    //        return true;
    //    }
    //}

    [Id("ObjectManager")]
    public class OutValue : Decision<ObjectManager>
    {
        public OutValue()
        {
            InVal.Add(new Dictval("value", typeof(string)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            var x=GetInputVal<string>(workProcess, "value");
            Debug.Log(x);
            result = "succ";
            return true;
        }
    }

    [Id("ObjectManager")]
    public class Conversation : Decision<ObjectManager>
    {
        [Show]
        public string inType ;
        [Show]
        public string outType ;
        public Conversation()
        {

        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            var inval=GetInputVal(workProcess, "In");
            setDictVal(workProcess, "Out",inval);
            result = "succ";
            return true;
        }
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            Type inv= BasicFunction.GetTypeByName(inType);
            Type outv = BasicFunction.GetTypeByName(outType);
            var res=new Pair<List<string>, List<Dictval>>();
            res.K2 = new List<Dictval>();
            res.K2.Add(new Dictval("Out",outv));
            res.K3 = new List<Dictval>();
            res.K3.Add(new Dictval("In", inv));
            return res;
        }
    }

    /// <summary>
    /// 暂停回合
    /// </summary>
    [Id("ObjectManager")]
    public class Stop : Decision<ObjectManager>
    {
        public Stop()
        {
        }
        private int nowtime = 0;
        public override void Begin(WorkProcess workProcess)
        {
        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            //Debug.Log("stop");
            workProcess.stopStage = true;
            result = "succ";
            return true;
        }
    }
    
    [Id("ObjectManager")]
    public class ObjectMap : Decision<ObjectManager>
    {
        public ObjectMap()
        {
            InVal.Add(new Dictval("obj",typeof(object)));
        }
        [Show]
        public string type;
        public override void Begin(WorkProcess workProcess)
        {
        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            //Debug.Log(workProcess.workAsset.name);
            //Debug.Log(type);
            Type temp = BasicFunction.GetTypeByName(type);
            var x=GetInputVal(workProcess, "obj");
            foreach(var pi in temp.GetFields())
            {
                if(pi.IsPublic)//是否为共有
                {
                    setDictVal(workProcess, pi.Name+"(V)", pi.GetValue(x));
                }
            }
            result = "succ";
            return true;
        }
        public override Pair<List<string>, List<Dictval>> UpdateOutPort()
        {
            var res=new Pair<List<string>, List<Dictval>>();
            res.K1 = null;
            res.K3 = new List<Dictval>();
            res.K3.Add(new Dictval("obj", typeof(object)));
            res.K2 = new List<Dictval>();

            Type temp = BasicFunction.GetTypeByName(type);
            foreach (var pi in temp.GetFields())
            {
                if (pi.IsPublic)//是否为共有
                {
                    res.K2.Add(new Dictval(pi.Name+"(V)",pi.FieldType));
                }
            }
            return res;
        }
    }
    //根据对象监听
    public class Listen : Decision<ObjectManager>
    {
        public Listen()
        {
            InVal.Add(new Dictval("jud",typeof(AllowMethod)));
            InVal.Add(new Dictval("content", typeof(string)));
            Out.Add("fail");
        }
        public override void Begin(WorkProcess workProcess)
        {
        }
        public override void Execute(WorkProcess workProcess)
        {

        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            var x=GetInputVal<AllowMethod>(workProcess, "jud");
            if(x==null)
            {
                result = "succ";
            }
            else
            {
                var j=x( GetInputVal<string>(workProcess,"content") );
                if(j==true)
                {
                    result = "succ";
                }
                else
                {
                    result = "fail";
                }
            }
            return true;
        }
    }
    [Id("ObjectManager")]
    public class IsNull : Decision<ObjectManager>
    {
        public IsNull()
        {
            Out.Clear();
            Out.Add("is null");
            Out.Add("not null");
            OutVal.Add(new Dictval("jud",typeof(bool)));
            InVal.Add(new Dictval("res", typeof(object)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            var t=GetInputVal(workProcess, "res");
            //Debug.Log(1111111111);
            if (t == null)
            {
                result = "is null";
                setDictVal(workProcess, "jud", false);
            }
            else
            {
                result = "not null";
                setDictVal(workProcess, "jud", true);
            }
            return true;
        }
    }

    [Id("ObjectManager")]
    public class RandomCheck : Decision<ObjectManager>
    {
        /// <summary>
        /// 判断值
        /// </summary>
        [Show]
        public float floorVal;
        public RandomCheck()
        {
            Out.Add("fail");
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            if(UnityEngine.Random.Range(0.0f,1.0f)<=floorVal)
            {
                result = "succ";
            }
            else
            {
                result = "fail";
            }
            return true;
        }
    }



    [Id("ObjectManager")]
    public class OutVal<T> : Decision<ObjectManager>
    {
        [Show]
        public T val;
        public OutVal()
        {
            OutVal.Add(new Dictval("res", typeof(T)));
        }
        public override void Begin(WorkProcess workProcess)
        {

        }
        public override void Execute(WorkProcess workProcess)
        {
        }
        public override bool HaveEnd(WorkProcess workProcess, out string result)
        {
            setDictVal(workProcess, "res", val);
            result = "succ";
            return true;
        }
    }



    [Id("ObjectManager")]
    public class OutBool:OutVal<bool>
    {

    }


    [Id("ObjectManager")]
    public class OutWorkAsset : OutVal<WorkAsset>
    {

    }
}
