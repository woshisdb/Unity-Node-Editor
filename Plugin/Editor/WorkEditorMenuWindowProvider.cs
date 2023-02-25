using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WorkEditorMenuWindowProvider : ScriptableObject,ISearchWindowProvider
{
    /// <summary>
    /// 所有的变量的类型
    /// </summary>
    Type[] VariableTypes = { typeof(int), typeof(float), typeof(Vector3) };
    public delegate bool SerchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry,SearchWindowContext context);

    public SerchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;                              //delegate回调方法
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        return EditorNodeManager.CreateSearchList();
    }
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        if (OnSelectEntryHandler == null)
        {
            return false;
        }
        return OnSelectEntryHandler(searchTreeEntry, context);
    }
}
