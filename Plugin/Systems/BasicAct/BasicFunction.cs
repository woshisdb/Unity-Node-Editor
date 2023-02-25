using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NodeEditor
{
    public class BasicFunction
    {
        public static T ToEnum<T>(string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }
        public static T DeepCopyByBinary<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        public static Type GetTypeByName(string typename)
        {
            Type t = null;
            string source = typename;
            if (source.IndexOf('<') > 0)
            {
                List<string> lv = new List<string>();
                while (Regex.IsMatch(source, @"<[^<>]+>"))
                {
                    lv.Add(Regex.Match(source, @"(?<=<)[^<>]+(?=>)").Value);
                    source = Regex.Replace(source, @"<[^<>]+>", "/" + (lv.Count - 1));
                }
                List<Type[]> args = new List<Type[]>();
                for (int i = 0; i < lv.Count; i++)
                {
                    List<Type> arg = new List<Type>();
                    string[] sp = lv[i].Split(',');
                    for (int j = 0; j < sp.Length; j++)
                    {
                        string s = sp[j].Trim();
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (Regex.IsMatch(s, @"/\d+$"))
                            {
                                Match m = Regex.Match(s, @"^([^/\s]+)\s*/(\d+)$");
                                if (!m.Success)
                                {
                                    throw new Exception("");
                                }
                                Type p = GetTypeByName(m.Groups[1].Value);
                                Type c = p.MakeGenericType(args[Convert.ToInt32(m.Groups[2].Value)]);
                                arg.Add(c);
                            }
                            else
                            {
                                arg.Add(GetTypeByName(s));
                            }
                        }
                    }
                    args.Add(arg.ToArray());
                }
                Match f = Regex.Match(source, @"^([^/\s]+)\s*/(\d+)$");
                if (!f.Success)
                {
                    throw new Exception("");
                }
                Type fp = GetTypeByName(f.Groups[1].Value);
                Type fc = fp.MakeGenericType(args[Convert.ToInt32(f.Groups[2].Value)]);
                return fc;
            }
            else
            {
                try
                {
                    t = Type.GetType(source);
                    if (t != null)
                    {
                        return t;
                    }
                    Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly ass in assembly)
                    {
                        t = ass.GetType(source);
                        if (t != null)
                        {
                            return t;
                        }
                        Type[] ts = ass.GetTypes();
                        foreach (Type st in ts)
                        {
                            if (Regex.IsMatch(st.FullName, @"\." + source + @"(`?\d+)?$"))
                            {
                                return st;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return t;
        }
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            //Debug.Log(nameSpace);
            var res = assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
            //Debug.Log(res.Length);
            return res;
        }
        /// <summary>
        /// 判断子类关系
        /// </summary>
        /// <param name="baseClass"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        public static bool IsSubClass(Type baseClass, Type subClass)
        {
            if (baseClass.IsAssignableFrom(subClass) == true || (subClass.BaseType != null && IsSubClass(baseClass, subClass.BaseType) == true))
            {
                return true;
            }
            else
                return false;
        }
        public static object ConvertS(object value, Type conversionType)
        {
            if (value == null || value is DBNull)
                return null;
            if (conversionType.IsInstanceOfType(value))
                return value;
            if (conversionType.IsEnum)
            {
                var s = value as string;
                if (s != null)
                {
                    return Enum.Parse(conversionType, s);
                }
                return Enum.ToObject(conversionType, value);
            }
            var t = Nullable.GetUnderlyingType(conversionType) ?? conversionType;
            return System.Convert.ChangeType(value, t);
        }

        public static bool IsGenericList(Type type)
        {
            if (!type.IsGenericType) { return false; }

            var typeDef = type.GetGenericTypeDefinition();
            if (typeDef == typeof(List<>) || typeDef == typeof(IList<>)) { return true; }
            return false;
        }
    }

    /// <summary>
    /// 允许使用的方法,监听当前的选项是否允许
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate bool AllowMethod(string instance);
}