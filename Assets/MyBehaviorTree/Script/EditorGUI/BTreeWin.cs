﻿#if UNITY_EDITOR
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Belong.BehaviorTree;

namespace Belong.BehaviorTree.Editor
{

    public class NodeEditor
    {
        public BNode CurNode { get; set; }
        public Rect CurRect { get; set; }
        public Rect CurLabRect { get; set; }
    }
    public class BTreeWin : EditorWindow
    {
        public static int NODE_WIDTH = 20;
        public static int NODE_HEIGHT = 20;
        public static int GUI_WIDTH = 240;

        public static BTree cur_tree;	//current tree
        public static BNode cur_node;	//current node
        public static BTreeWin sInstance = null;
        private static int cur_tree_index = -1;
        private static int last_tree_index = -1;
        private static int select_create_node_id = -1;

        public static BNode select;

        //temp value
        private Vector2 m_cScrollPos = new Vector2(0, 0);
        private string m_strInputName = "";

        public static List<Type> m_lstComposite = new List<Type>();
        public static List<Type> m_lstAction = new List<Type>();
        public static List<Type> m_lstCondition = new List<Type>();
        public static List<Type> m_lstDecorator = new List<Type>();

        public List<NodeEditor> m_lstNodeEditor = new List<NodeEditor>();

        [@MenuItem("BTree/Editor")]
        static void initwin()
        {
            BTreeWin win = (BTreeWin)BTreeWin.GetWindow(typeof(BTreeWin));
            sInstance = win;
            sInstance.titleContent = new GUIContent("树编辑");
            m_lstComposite = GetNodeType(typeof(BNodeComposite));
            m_lstAction = GetNodeType(typeof(BNodeAction));
            m_lstCondition = GetNodeType(typeof(BNodeCondition));
            m_lstDecorator = GetNodeType(typeof(BNodeDecorator));
        }

        public static void addnode(object arg)
        {
            Debug.Log("callback " + arg);
        }

        void OnGUI()
        {
            //////////////////// draw the tree /////////////////////
            //this.m_cScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width - 240, position.height), this.m_cScrollPos, new Rect(0, 0, this.maxSize.x, this.maxSize.y));
            this.m_cScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width - 240, position.height), this.m_cScrollPos, new Rect(0, 0, this.position.width - GUI_WIDTH - 15, this.maxSize.y));

            Texture2D tex1 = new Texture2D(1, 1);
            //tex1.SetPixel(0, 0, Color.black);
            tex1.SetPixel(0, 0, Color.cyan);
            tex1.Apply();
            Texture2D tex2 = new Texture2D(1, 1);
            tex2.SetPixel(0, 0, Color.gray);
            tex2.Apply();
            for (int i = 0; i < 1000; i++)
            {
                if (i % 2 == 0)
                    GUI.DrawTexture(new Rect(0, i * NODE_HEIGHT, this.position.width - GUI_WIDTH - 15, NODE_HEIGHT), tex1);
                else
                    GUI.DrawTexture(new Rect(0, i * NODE_HEIGHT, this.position.width - GUI_WIDTH - 15, NODE_HEIGHT), tex2);
            }

            if (cur_tree != null && cur_tree.m_cRoot != null)
            {
                m_lstNodeEditor.Clear();
                int xx = 0;
                int yy = 0;
                RenderNode(cur_tree.m_cRoot, xx, ref yy);
                //cur_tree.m_cRoot.Render(xx, ref yy);
            }
            RenderNodeContext();

            GUI.EndScrollView();
            //////////////////// draw the tree /////////////////////

            //////////////////// draw editor gui /////////////////////
            GUI.BeginGroup(new Rect(position.width - GUI_WIDTH, 0, 300, 1000));
            int x = 0;
            int y = 0;
            List<BTree> lst = BTreeMgr.sInstance.GetTrees();
            if (GUI.Button(new Rect(x, y, 200, 40), "Load"))
            {
                cur_tree = null;
                cur_node = null;
                cur_tree_index = -1;
                last_tree_index = -1;
                select_create_node_id = -1;
                select = null;
                EditorLoad();
                //BTreeMgr.sInstance.EditorLoad();
            }
            y += 40;
            if (GUI.Button(new Rect(x, y, 200, 40), "Save Editor BTree"))
            {
                EditorSave();
                //BTreeMgr.sInstance.EditorSave();
                AssetDatabase.Refresh();
            }
            //y += 40;
            //if (GUI.Button(new Rect(x, y, 200, 40), "Save BTree"))
            //{
            //    //			BTreeMgr.sInstance.SaveEx();
            //    AssetDatabase.Refresh();
            //}
            y += 40;
            GUI.Label(new Rect(x, y, 200, 20), "=======================");
            y += 20;

            this.m_strInputName = GUI.TextField(new Rect(x, y + 10, 100, 20), this.m_strInputName);
            if (GUI.Button(new Rect(x + 100, y, 100, 40), "create tree"))
            {
                if (this.m_strInputName != "")
                {
                    cur_node = null;
                    BTree tree = new BTree();
                    tree.m_strName = this.m_strInputName;
                    BTreeMgr.sInstance.Add(tree);
                    lst = BTreeMgr.sInstance.GetTrees();
                    cur_tree = tree;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].m_strName == tree.m_strName)
                        {
                            cur_tree_index = i;
                            break;
                        }
                    }
                    last_tree_index = cur_tree_index;
                    Repaint();
                }
            }
            y += 40;
            if (GUI.Button(new Rect(x, y, 200, 40), "remove tree"))
            {
                cur_node = null;
                BTreeMgr.sInstance.Remove(cur_tree);
                lst = BTreeMgr.sInstance.GetTrees();
                cur_tree = null;
                cur_tree_index = -1;
                last_tree_index = -1;
                Repaint();
            }
            y += 40;
            GUI.Label(new Rect(x, y, 200, 20), "=======================");
            y += 20;

            string[] treeNames = new string[lst.Count];
            for (int i = 0; i < lst.Count; i++)
            {
                treeNames[i] = lst[i].m_strName;
            }
            cur_tree_index = EditorGUI.Popup(new Rect(x, y, 200, 45), cur_tree_index, treeNames);
            if (cur_tree_index != last_tree_index)
            {
                last_tree_index = cur_tree_index;
                cur_tree = lst[cur_tree_index];
                cur_node = null;
            }
            y += 40;
            GUI.Label(new Rect(x, y, 200, 20), "=======================");
            y += 20;

            if (cur_tree != null)
            {
                GUI.Label(new Rect(x, y, 200, 20), "TreeName: " + cur_tree.m_strName);
                y += 20;
                cur_tree.m_strName = GUI.TextField(new Rect(x, y, 100, 20), cur_tree.m_strName);
                y += 20;
            }
            //select_create_node_id = EditorGUI.Popup(new Rect(x, y, 100, 40), select_create_node_id, BNodeFactory.sInstance.GetNodeLst());
            select_create_node_id = EditorGUI.Popup(new Rect(x, y, 100, 40), select_create_node_id, GetNodeLst());
            if (GUI.Button(new Rect(x + 100, y, 100, 40), "create root"))
            {
                if (select_create_node_id >= 0)
                {
                    //BNode node = BNodeFactory.sInstance.Create(select_create_node_id);
                    BNode node = Create(select_create_node_id);
                    if (cur_tree != null)
                        cur_tree.m_cRoot = node;
                }
            }
            y += 40;
            if (GUI.Button(new Rect(x, y, 200, 40), "clear"))
            {
                if (cur_tree != null)
                    cur_tree.Clear();
            }
            y += 40;
            GUI.Label(new Rect(x, y, 200, 20), "=======================");
            y += 20;
            if (select != null)
            {
                GUI.Label(new Rect(x, y, 300, 20), "Node Type: " + select.GetType().FullName);
                y += 20;
                GUI.Label(new Rect(x, y, 200, 20), "Node Name: " + select.GetName());
                y += 20;
                GUI.Label(new Rect(x, y, 200, 15), "=======================");
                y += 15;
                select.RenderEditor(x, y);
            }
            //
            GUI.EndGroup();
            //////////////////// draw editor gui /////////////////////
        }

        void Update()
        {
            sInstance = this;
            if (select != null)
            {
                Repaint();
            }
          
        }



        #region 自定义方法
        /// <summary>
        /// 保存编辑器文件
        /// </summary>

        public void EditorSave()
        {
            string filepath = EditorUtility.SaveFilePanel("Behavior Tree", Application.dataPath, "", "json");
            Debug.Log(filepath);
            JsonData data = new JsonData();
            data["trees"] = new JsonData();
            data["trees"].SetJsonType(JsonType.Array);
            foreach (KeyValuePair<string, BTree> item in BTreeMgr.sInstance.m_mapTree)
            {
                item.Value.WriteJson(data["trees"]);
            }
            File.WriteAllText(filepath, data.ToJson());
        }

        /// <summary>
        /// 读取编辑器文件
        /// </summary>
        public void EditorLoad()
        {
            string filepath = EditorUtility.OpenFilePanel("Bahvior Tree", Application.dataPath, "json");
            if (filepath == "") return;
            BTreeMgr.sInstance.m_mapTree.Clear();
            string txt = File.ReadAllText(filepath);
            BTreeMgr.sInstance.Load(txt);
        }

        public void RenderNode(BNode node, int x, ref int y)
        {
            Event evt = Event.current;
            string showName = string.Empty;
            object[] attrobjs = null;
            showName = node.Name;
            attrobjs = node.GetType().GetCustomAttributes(typeof(BClassAttribute), false);
            for (int i = 0; i < attrobjs.Length; i++)
            {
                BClassAttribute b = attrobjs[i] as BClassAttribute;
                if (b != null && !string.IsNullOrEmpty(b.ShowName))
                {
                    showName = b.ShowName;
                }
            }
            GUI.Label(new Rect(x, y, sInstance.position.width, NODE_HEIGHT), showName);
            NodeEditor ne = new NodeEditor();
            ne.CurNode = node;
            ne.CurRect = new Rect(0, y, sInstance.position.width - GUI_WIDTH, NODE_HEIGHT);
            ne.CurLabRect = new Rect(x, y, sInstance.position.width, NODE_HEIGHT);
            m_lstNodeEditor.Add(ne);
            /////////////////// line //////////////////////
            //Vector3 pos1 = new Vector3(x + NODE_WIDTH / 2, y + NODE_HEIGHT, 0);
            Handles.color = Color.red;
            if (node != null && node.ChildList != null)
            {
                for (int i = 0; i < node.ChildList.Count; i++)
                {
                    y = y + NODE_HEIGHT;

                    //Vector3 pos2 = new Vector3(x + NODE_WIDTH / 2, y + NODE_HEIGHT / 2, 0);
                    //Vector3 pos3 = new Vector3(x + NODE_WIDTH, y + NODE_HEIGHT / 2, 0);
                    RenderNode(node.ChildList[i], x + NODE_WIDTH, ref y);
                    //node.ChildList[i].Render(x + NODE_WIDTH, ref y);
                    //Handles.DrawPolyLine(new Vector3[] { pos1, pos2, pos3 });
                }
            }

        }


        //menu add decision node

        //public void RenderNode(BNode node, int x, ref int y)
        //{
        //    Event evt = Event.current;
        //    Debug.LogError(evt.mousePosition);
        //    string showName = string.Empty;
        //    object[] attrobjs = null;
        //    if (cur_node == node)
        //    {
        //        Texture2D texRed = new Texture2D(1, 1);
        //        texRed.SetPixel(0, 0, Color.blue);
        //        texRed.Apply();
        //        GUI.DrawTexture(new Rect(0, y, sInstance.position.width, BTreeWin.NODE_HEIGHT), texRed);
        //    }

        //    Rect moveRect = new Rect(0, y, sInstance.position.width - GUI_WIDTH, 5);
        //    bool is_move_node = false;
        //    if (select != null && moveRect.Contains(evt.mousePosition))
        //    {
        //        is_move_node = true;
        //        Texture2D tex = new Texture2D(1, 1);
        //        tex.SetPixel(0, 0, Color.green);
        //        tex.Apply();
        //        GUI.DrawTexture(new Rect(0, y, sInstance.position.width - GUI_WIDTH, 2), tex);
        //        if (evt.button == 0 && evt.type == EventType.MouseUp)
        //        {
        //            if (node != select && node.Parent != null)
        //            {
        //                select.Parent.RemoveChild(select);
        //                select.Parent = node.Parent;
        //                node.Parent.InsertChild(node, select);
        //            }
        //            select = null;
        //            sInstance.Repaint();
        //        }
        //    }

        //    Rect rect = new Rect(0, y, sInstance.position.width - GUI_WIDTH, NODE_HEIGHT);
        //    if (!is_move_node && rect.Contains(evt.mousePosition))
        //    {
        //        if (select != null)
        //        {
        //            Texture2D texRed = new Texture2D(1, 1);
        //            texRed.SetPixel(0, 0, Color.red);
        //            texRed.Apply();
        //            GUI.DrawTexture(new Rect(0, y, sInstance.position.width - GUI_WIDTH, NODE_HEIGHT), texRed);
        //        }
        //        if (evt.type == EventType.ContextClick)
        //        {
        //            GenericMenu menu = new GenericMenu();
        //            foreach (Type item in m_lstComposite)
        //            {
        //                menu.AddItem(new GUIContent("Create/Composite/" + item.Name), false, menu_add_callback, new object[] { node, item });
        //            }

        //            foreach (Type item in m_lstAction)
        //            {
        //                menu.AddItem(new GUIContent("Create/Action/" + item.Name), false, menu_add_callback, new object[] { node, item });
        //            }
        //            foreach (Type item in m_lstCondition)
        //            {
        //                menu.AddItem(new GUIContent("Create/Condition/" + item.Name), false, menu_add_callback, new object[] { node, item });
        //            }
        //            foreach (Type item in m_lstDecorator)
        //            {
        //                menu.AddItem(new GUIContent("Create/Decorator/" + item.Name), false, menu_add_callback, new object[] { node, item });
        //            }

        //            foreach (Type item in m_lstComposite)
        //            {
        //                menu.AddItem(new GUIContent("Switch/Composite/" + item.Name), false, menu_switch_callback, new object[] { node, item });
        //            }

        //            menu.AddItem(new GUIContent("Delete"), false, menu_delete_node, node);
        //            menu.ShowAsContext();
        //        }
        //        if (evt.button == 0 && evt.type == EventType.MouseDown && node != cur_tree.m_cRoot)
        //        {
        //            select = node;
        //            cur_node = node;
        //        }
        //        if (evt.button == 0 && evt.type == EventType.MouseUp && select != null)
        //        {
        //            if (node != select)
        //            {
        //                select.Parent.RemoveChild(select);
        //                select.Parent = node;
        //                node.AddChild(select);
        //            }
        //            select = null;
        //            sInstance.Repaint();
        //        }
        //    }
        //    showName = node.Name;
        //    attrobjs = node.GetType().GetCustomAttributes(typeof(BClassAttribute), false);
        //    for (int i = 0; i < attrobjs.Length; i++)
        //    {
        //        BClassAttribute b = attrobjs[i] as BClassAttribute;
        //        if (b != null && !string.IsNullOrEmpty(b.ShowName))
        //        {
        //            showName = b.ShowName;
        //        }
        //    }
        //    GUI.Label(new Rect(x, y, sInstance.position.width, NODE_HEIGHT), showName);

        //    /////////////////// line //////////////////////
        //    //Vector3 pos1 = new Vector3(x + NODE_WIDTH / 2, y + NODE_HEIGHT, 0);
        //    Handles.color = Color.red;
        //    if (node != null && node.ChildList != null)
        //    {
        //        for (int i = 0; i < node.ChildList.Count; i++)
        //        {
        //            y = y + NODE_HEIGHT;

        //            //Vector3 pos2 = new Vector3(x + NODE_WIDTH / 2, y + NODE_HEIGHT / 2, 0);
        //            //Vector3 pos3 = new Vector3(x + NODE_WIDTH, y + NODE_HEIGHT / 2, 0);
        //            RenderNode(node.ChildList[i], x + NODE_WIDTH, ref y);
        //            //node.ChildList[i].Render(x + NODE_WIDTH, ref y);
        //            //Handles.DrawPolyLine(new Vector3[] { pos1, pos2, pos3 });
        //        }
        //    }

        //}

        public void RenderNodeContext()
        {
            Event evt = Event.current;
            cur_node = null;
            int count = 0;
            for (int i = 0; i < m_lstNodeEditor.Count; i++)
            {
                if (m_lstNodeEditor[i].CurRect.Contains(evt.mousePosition))
                {
                    //cur_node = m_lstNodeEditor[i].CurNode;
                    if (evt.button == 0 && evt.isMouse)
                    {
                        select = m_lstNodeEditor[i].CurNode;
                    }
                }
                else
                {
                    if (evt.button == 0 && evt.isMouse)
                    {
                        count++;
                    }
                }
            }
            if (count >= m_lstNodeEditor.Count)
            {
                select = null;
                Repaint();
                return;
            }
            if (select != null)
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.red);
                tex.Apply();
                NodeEditor ne = m_lstNodeEditor.Find(a => a.CurNode == select);
                if (ne != null)
                {
                    GUI.DrawTexture(ne.CurRect, tex);
                    string showName = string.Empty;
                    object[] attrobjs = null;
                    showName = select.Name;
                    attrobjs = select.GetType().GetCustomAttributes(typeof(BClassAttribute), false);
                    for (int i = 0; i < attrobjs.Length; i++)
                    {
                        BClassAttribute b = attrobjs[i] as BClassAttribute;
                        if (b != null && !string.IsNullOrEmpty(b.ShowName))
                        {
                            showName = b.ShowName;
                        }
                    }
                    GUI.Label(ne.CurLabRect, showName);
                }
            }
        }



        //menu add decision node

        private void menu_add_callback(object arg)
        {
            object[] objs = arg as object[];
            BNode bnode = objs[0] as BNode;
            Type t = objs[1] as Type;
            BNode node = Activator.CreateInstance(t) as BNode;

            bnode.AddChild(node);
            node.Parent = bnode;
            sInstance.Repaint();
        }
        //menu switch node
        private void menu_switch_callback(object arg)
        {
            object[] objs = arg as object[];
            BNode bnode = objs[0] as BNode;
            Type t = objs[1] as Type;
            BNode node = Activator.CreateInstance(t) as BNode;
            node.Parent = bnode.Parent;
            foreach (BNode item in bnode.ChildList)
            {
                node.AddChild(item);
            }
            if (bnode.Parent != null)
            {
                bnode.Parent.ReplaceChild(bnode, node);
            }
            else if (cur_tree.m_cRoot == bnode)
            {
                cur_tree.m_cRoot = node;
            }
            cur_node = node;
            sInstance.Repaint();
        }

        //menu delete node
        private void menu_delete_node(object arg)
        {
            BNode node = arg as BNode;
            if (node.Parent != null)
            {
                node.Parent.RemoveChild(node);
            }
            node.Parent = null;
            select = null;
            cur_node = null;
            sInstance.Repaint();
        }

        private static List<Type> GetNodeType(Type nodeType)
        {
            List<Type> listType = new List<Type>();

            Type[] allTypes = typeof(BNode).Assembly.GetTypes();
            for (int i = 0; i < allTypes.Length; i++)
            {
                if (allTypes[i].IsSubclassOf(nodeType))
                {
                    listType.Add(allTypes[i]);
                }
            }
            return listType;
        }

        public string[] GetNodeLst()
        {
            string[] str = new string[m_lstComposite.Count];
            for (int i = 0; i < m_lstComposite.Count; i++)
            {
                Type item = m_lstComposite[i];
                str[i] = item.Name;
                object[] objs = item.GetCustomAttributes(typeof(BClassAttribute), false);
                for (int j = 0; j < objs.Length; j++)
                {
                    BClassAttribute b = objs[j] as BClassAttribute;
                    if (b != null)
                    {
                        if (!string.IsNullOrEmpty(b.ShowName))
                        {
                            str[i] = b.ShowName;
                        }
                    }
                }
            }
            return str;
        }

        public BNode Create(int index)
        {
            if (m_lstComposite.Count > index)
            {
                Type t = m_lstComposite[index];
                BNode node = Activator.CreateInstance(t) as BNode;
                return node;
            }
            Debug.LogError("The type index is none : " + index);
            return null;
        }

        #endregion

    }
}
#endif