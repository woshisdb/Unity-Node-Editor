using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;
using ObjectManagerAct;
using NodeEditor;

public class WorkEditor : EditorWindow
{
    WorkEditorView view;
    public static WorkAsset workAsset=null;
    private ObjectField objectField;
    //private ObjectField activityObjectField;
    private Label information;
    //private Label activityinf;
    [MenuItem("Tools/WorkEditor")]
    public static void ShowExample()
    {
        WorkEditor wnd = GetWindow<WorkEditor>();
        wnd.titleContent = new GUIContent("WorkEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugin/Editor/WorkEditor.uxml");
        visualTree.CloneTree(root);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugin/Editor/WorkEditor.uss");
        root.styleSheets.Add(styleSheet);
        Button loadButton = root.Q<Button>("Load");
        loadButton.RegisterCallback<MouseCaptureEvent>(Load);
        Button saveButton = root.Q<Button>("Save");
        saveButton.RegisterCallback<MouseCaptureEvent>(Save);
        Button reSizeButton = root.Q<Button>("ReSize");
        reSizeButton.RegisterCallback<MouseCaptureEvent>(ReSize);
        view =root.Q<WorkEditorView>("View");
        view.workEditor = this; 
        objectField = root.Q<ObjectField>("Asset");
        objectField.objectType = typeof(WorkAsset);
        information = root.Q<Label>("Information");
        workAsset = null;
    }
    void ReSize(MouseCaptureEvent evt)
    {
        view.FrameAll();
        //Debug.Log(view.FrameAll());
        ////view.Focus();
        //List<Node> nodes = view.nodes.ToList();
        //foreach(var node in nodes)
        //{
        //    Debug.Log(node.GetPosition());
        //}
        //Debug.Log(view.CalculateRectToFitAll(view));
    }
    void Load(MouseCaptureEvent evt)
    {
        //var path = AssetDatabase.GetAssetPath(workAsset);
        //UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        //for (int i = 0; i < assets.Length; i++)
        //{
        //    workAsset.nodes.Add(assets[i] as NodeAsset);
        //}
        workAsset =(WorkAsset)objectField.value;
        information.text = "NULL";
        if (workAsset!=null)
        {
            information.text = workAsset.name;
            view.DeleteElements(view.graphElements.ToList());
            for (int i=0;i<workAsset.nodes.Count;i++)
            {
                var node = workAsset.nodes[i].retNode(i);
                view.AddElement(node);//workAsset.nodes[i].retNode(i));
                //Debug.Log(node.GetPosition());
            }
            for(int i=0;i<workAsset.nodes.Count;i++)//����ÿ���ڵ�
            {
                foreach(var node in workAsset.nodes[i].output)
                {
                    if (node.Value != null)
                    {
                        Port outPort = view.Q<Node>(i + "").outputContainer.Q<Port>(node.Key);//��ǰ�ڵ�ĳ���
                        if (outPort != null)
                        {
                            //foreach (var pot in node.Value)
                            //{
                            var pot = node.Value;
                            Edge edge = new Edge();
                            edge.output = outPort;
                            Port inPort = view.Q<Node>(pot.NodeNo + "").inputContainer.Q<Port>(pot.PortName);//��ǰ�ڵ�ĳ���
                            if (inPort != null)
                            {
                                edge.input = inPort;
                                edge?.input.Connect(edge);
                                edge?.output.Connect(edge);
                                view.AddElement(edge);
                                //}
                            }
                        }
                    }
                }
                foreach (var node in workAsset.nodes[i].inVal)
                {
                    if (node.Value != null)
                    {
                        Port inPort = view.Q<Node>(i + "").inputContainer.Q<Port>(node.Key);//��ǰ�ڵ�ĳ���
                        if (inPort != null)
                        {
                            //foreach (var pot in node.Value)
                            //{
                            var pot = node.Value;
                            Edge edge = new Edge();
                            edge.input = inPort;
                            Port outPort = view.Q<Node>(pot.NodeNo + "").outputContainer.Q<Port>(pot.PortName);//��ǰ�ڵ�ĳ���
                            if (outPort != null)
                            {
                                edge.output = outPort;
                                edge?.input.Connect(edge);
                                edge?.output.Connect(edge);
                                view.AddElement(edge);
                            }
                            //}
                        }
                    }
                }
            }
            foreach (GroupAsset g in workAsset.groups)
            {
                Group group = new Group();
                group.title= g.title;
                foreach(int j in g.nodesNos)
                {
                    Node n= view.Q<Node>(j+"");
                    group.AddElement(n);
                    view.AddElement(group);
                }
            }
            //Debug.Log(view.contentRect);
        }
    }
    void Save(MouseCaptureEvent evt)
    {
        if (workAsset!=null)
        {
            var path = AssetDatabase.GetAssetPath(workAsset);
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                DestroyImmediate(assets[i], true);
            }
            workAsset.nodes.Clear();
            ////////////////////////////////
            workAsset.nodes=null;
            List<NodeAsset> nodeAssets = new List<NodeAsset>();//������ʼ����Դ
            List<Node> nodes = view.nodes.ToList();
            List<Edge> edges = view.edges.ToList();
            workAsset.beginNo = -1;
            workAsset.endNo = -1;
            workAsset.breakNo = -1;
            workAsset.judgeNo = -1;
            workAsset.EnvirNo = -1;
            workAsset.dictvals.Clear();
            //���һϵ��no
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                node.name = i+"";
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeAsset nodeAsset;

                //��ʼ�ڵ�
                if (nodes[i].GetType().Name==typeof(BeginNode).Name)
                {
                    workAsset.beginNo = int.Parse(nodes[i].name);//��ǿ�ʼ�ڵ�
                }
                //�����ڵ�
                else if(nodes[i].GetType().Name==typeof(EndNode).Name)
                {
                    workAsset.endNo = int.Parse(nodes[i].name);//��ʶ�����ڵ�
                }
                else if (nodes[i].GetType().Name == typeof(DecisionNode).Name)
                {
                    if(((DecisionNode)nodes[i]).decisionPort.DecisionType == typeof(Envirment)||BasicFunction.IsSubClass(typeof(Envirment), ((DecisionNode)nodes[i]).decisionPort.DecisionType))
                    {
                        var ds = (DecisionNode)nodes[i];
                        workAsset.dictvals = ((DecisionNode)nodes[i]).nowAction.UpdateOutPort().K2;
                        workAsset.EnvirNo = i;//������λ��

                    }
                }
                BaseNode baseNode = nodes[i] as BaseNode;
                nodeAsset=baseNode.GenerateNodeAsset(baseNode, workAsset);
                AssetDatabase.AddObjectToAsset(nodeAsset, workAsset);
                nodeAsset.no = i;
                nodeAssets.Add(nodeAsset);
            }
            //����ߵ���Ϣ
            for (int i=0;i<edges.Count;i++)
            {
                int inter =int.Parse(edges[i].input.node.name);
                int outer= int.Parse(edges[i].output.node.name);
                string outstr = edges[i].output.portName;
                string instr = edges[i].input.portName;
                if (edges[i].input.portType==typeof(Node))//�ǽڵ�
                {
                    //nodeAssets[inter].input[instr].Add(new PortAsset(outstr, outer, edges[i].output.portType));
                    nodeAssets[outer].output[outstr]=new PortAsset(instr, inter, edges[i].input.portType);
                }
                else
                {
                    //Debug.Log(inter+","+instr+":"+outer+","+outstr);
                    nodeAssets[inter].inVal[instr]=new PortAsset(outstr, outer, edges[i].output.portType);
                    //nodeAssets[outer].outVal[outstr].Add(new PortAsset(instr, inter, edges[i].input.portType));
                }
            }
            List<GraphElement> groups = view.graphElements.ToList();
            ///һϵ�е�Group
            workAsset.groups = new List<GroupAsset>();
            for (int i=0;i< groups.Count; i++)
            {
                if (groups[i].GetType()==typeof(Group))
                {
                    //Debug.Log(1);
                    //��ǰ����ֵ
                    int nums= workAsset.groups.Count;
                    groups[i].name = nums+"";
                    workAsset.groups.Add(new GroupAsset(groups[i] as Group));
                }
            }
            workAsset.nodes = nodeAssets;
            //���洴������Դ
            AssetDatabase.SaveAssets();
            //ˢ�½���
            AssetDatabase.Refresh();
        }
    }
}