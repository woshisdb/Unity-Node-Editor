using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkEditorView : GraphView
{
    public WorkEditor workEditor;
    public new class UxmlFactory : UxmlFactory<WorkEditorView, GraphView.UxmlTraits> { }
    public WorkEditorView()
    {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());//Assets/Scripts/GameEditors/Editor/WorkEditor.uxml
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugin/Editor/WorkEditor.uss");
        styleSheets.Add(styleSheet);
        var menuWindowProvider = ScriptableObject.CreateInstance<WorkEditorMenuWindowProvider>();
        menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;
        nodeCreationRequest += context =>
        {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
        };
    }
    /// <summary>
    /// 菜单的重载方法
    /// </summary>
    /// <param name="searchTreeEntry"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = GetLocalMousePositio(context.screenMousePosition, true);
        GraphElement temp = EditorNodeManager.CreateNodeMenu((GroupEntity)searchTreeEntry.userData, localMousePosition);
        AddElement(temp);
        return true;
    }

    public Vector2 GetLocalMousePositio(Vector2 mousePosition,bool isSearchWindow=false)
    {
        Vector2 worldMousePosition = mousePosition;
        if (isSearchWindow)
        {
            worldMousePosition -= workEditor.position.position;
        }    
        Vector2 localMousePOsition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePOsition;
    }
    /// <summary>
    /// 节点的链接重载
    /// </summary>
    /// <param name="startAnchor"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns></returns>
    public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        foreach (var port in ports.ToList())
        {
            if (
                startAnchor.direction == port.direction || 
                (startAnchor.portType != port.portType && startAnchor.portType.IsCastableTo(port.portType)==false&& port.portType.IsCastableTo(startAnchor.portType) == false))
            {
                continue;
            }
            compatiblePorts.Add(port);
        }
        return compatiblePorts;
    }
    
}
