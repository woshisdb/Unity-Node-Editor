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
    /// ���еı���������
    /// </summary>
    Type[] VariableTypes = { typeof(int), typeof(float), typeof(Vector3) };
    public delegate bool SerchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry,SearchWindowContext context);

    public SerchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;                              //delegate�ص�����
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
