/**
 * Belong
 * 2016.09.15
 */
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
#if UNITY_EDITOR
using UnityEditor;
using Belong.BehaviorTree.Editor;
#endif

namespace Belong.BehaviorTree
{
    public enum ActionResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS,
        /// <summary>
        /// 正在运行
        /// </summary>
        RUNNING,
        /// <summary>
        /// 失败
        /// </summary>
        FAILURE,
        /// <summary>
        /// 没有运行,Normal状态
        /// </summary>
        NONE
    }
    /// <summary>
    /// 行为树所有节点的父类
    /// </summary>
    public class BNode
    {

        #region 成员变量

        /// <summary>
        /// 当前类的类型，用于初始化反射
        /// </summary>
        [BHideField]
        public string MyType = string.Empty;
        /// <summary>
        /// 名字
        /// </summary>
        [BHideField]
        public string Name = "Node";
        /// <summary>
        /// 父节点
        /// </summary>
        [BHideField]
        public BNode Parent = null;
        /// <summary>
        /// 子节点
        /// </summary>
        [BHideField]
        public List<BNode> ChildList = new List<BNode>();

        private ActionResult m_eState;

        #endregion

        #region 构造函数

        public BNode()
        {
            this.Name = this.GetType().Name;
            this.MyType = this.GetType().FullName;
            this.ChildList = new List<BNode>();
        }

        #endregion

        #region 公共方法

        public void ReadJson(string json)
        {
            ReadJson(JsonMapper.ToObject(json));
        }

        /// <summary>
        /// 读json
        /// </summary>
        /// <param name="json"></param>
        public void ReadJson(JsonData json)
        {
            this.MyType = json["type"].ToString();
            this.Name = json["Name"].ToString();
            JsonData arg = json["arg"];
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty);
            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                FieldInfo info = fieldInfos[i];
                if (arg.Keys.Contains(info.Name))
                {
                    string str = arg[info.Name].ToString();
                    object val = null;
                    if (info.FieldType == typeof(int)) val = int.Parse(str);
                    else if (info.FieldType == typeof(float)) val = float.Parse(str);
                    else if (info.FieldType == typeof(bool)) val = bool.Parse(str);
                    else if (info.FieldType == typeof(string)) val = str;
                    info.SetValue(this, val);
                }
            }
            for (int i = 0; i < json["child"].Count; i++)
            {
                string typename = json["child"][i]["type"].ToString();
                Type tt = Type.GetType(typename);
                BNode enode = Activator.CreateInstance(tt) as BNode;
                enode.ReadJson(json["child"][i]);
                enode.Parent = this;
                this.AddChild(enode);
            }
        }
        /// <summary>
        /// 写json，编辑器状态下用的较多
        /// </summary>
        /// <returns></returns>
        public JsonData WriteJson()
        {
            JsonData json = new JsonData();
            json["type"] = this.MyType;
            json["name"] = this.Name;

            json["arg"] = new JsonData();
            json["arg"].SetJsonType(JsonType.Object);
            Type t = this.GetType();
            FieldInfo[] fieldInfos = t.GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo info = fieldInfos[i];
                object[] objs = info.GetCustomAttributes(typeof(BHideFieldAttribute),false);
                bool isBreak = false;
                for (int j = 0; j < objs.Length; j++)
                {
                    BHideFieldAttribute bh = objs[j] as BHideFieldAttribute;
                    if (bh!=null)
                    {
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    continue;
                }
                json["arg"][info.Name] = info.GetValue(this).ToString();
            }

            json["child"] = new JsonData();
            json["child"].SetJsonType(JsonType.Array);
            for (int i = 0; i < this.ChildList.Count; i++)
            {
                JsonData child = this.ChildList[i].WriteJson();
                json["child"].Add(child);
            }
            return json;
        }

        /// <summary>
        /// 运行节点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ActionResult RunNode(object input)
        {
            if (this.m_eState == ActionResult.NONE)
            {
                this.OnEnter(input);
                this.m_eState = ActionResult.RUNNING;
            }
            ActionResult res = this.Excute(input);
            if (res != ActionResult.RUNNING)
            {
                this.OnExit(input);
                this.m_eState = ActionResult.NONE;
            }
            return res;
        }

        /// <summary>
        /// 当前节点进入操作
        /// </summary>
        /// <param name="input"></param>
        public virtual void OnEnter(object input)
        {
            //
        }
        /// <summary>
        /// 当前节点执行操作
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual ActionResult Excute(object input)
        {
            return ActionResult.SUCCESS;
        }
        /// <summary>
        /// 当前节点退出时候的操作
        /// </summary>
        /// <param name="input"></param>
        public virtual void OnExit(object input)
        {
            //
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public string GetTypeName()
        {
            return this.MyType;
        }
        /// <summary>
        /// 设置类型
        /// </summary>
        /// <param name="type"></param>
        public void SetTypeName(string type)
        {
            this.MyType = type;
        }
        /// <summary>
        /// 获取当前类的名称
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.Name;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChild(BNode node)
        {
            this.ChildList.Remove(node);
        }
        /// <summary>
        /// 增加节点
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(BNode node)
        {
            this.ChildList.Add(node);
        }
        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="prenode"></param>
        /// <param name="node"></param>
        public void InsertChild(BNode prenode, BNode node)
        {
            int index = this.ChildList.FindIndex((a) => { return a == prenode; });
            this.ChildList.Insert(index, node);
        }
        /// <summary>
        /// 替换节点
        /// </summary>
        /// <param name="prenode"></param>
        /// <param name="node"></param>
        public void ReplaceChild(BNode prenode, BNode node)
        {
            int index = this.ChildList.FindIndex((a) => { return a == prenode; });
            this.ChildList[index] = node;
        }
        /// <summary>
        /// 是否存在当前节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool ContainChild(BNode node)
        {
            return this.ChildList.Contains(node);
        }

        #endregion


        #region Editor界面

#if UNITY_EDITOR
        //render editor
        public void RenderEditor(int x, int y)
        {
            try
            {
                Type t = this.GetType();
                FieldInfo[] fieldInfos = t.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo info = fieldInfos[i];
                    object[] objs = info.GetCustomAttributes(typeof(BHideFieldAttribute), false);
                    bool isBreak = false;
                    for (int j = 0; j < objs.Length; j++)
                    {
                        BHideFieldAttribute bh = objs[j] as BHideFieldAttribute;
                        if (bh != null)
                        {
                            isBreak = true;
                            break;
                        }
                    }
                    if (isBreak)
                    {
                        continue;
                    }
                    //
                    object vl = null;
                    if (info.FieldType == typeof(int))
                    {
                        string fieldvalue = info.GetValue(this).ToString();
                        GUI.Label(new Rect(x, y + i * 20, 100, 20), info.Name);
                        fieldvalue = GUI.TextField(new Rect(x + 100, y + i * 20, 100, 20), fieldvalue);
                        vl = int.Parse(fieldvalue);
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        string fieldvalue = info.GetValue(this).ToString();
                        GUI.Label(new Rect(x, y + i * 20, 100, 20), info.Name);
                        fieldvalue = GUI.TextField(new Rect(x + 100, y + i * 20, 100, 20), fieldvalue);
                        vl = float.Parse(fieldvalue);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        bool fieldvalue = (bool)info.GetValue(this);
                        GUI.Label(new Rect(x, y + i * 20, 100, 20), info.Name);
                        fieldvalue = GUI.Toggle(new Rect(x + 100, y + i * 20, 100, 20), fieldvalue, "");
                        vl = fieldvalue;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        string fieldvalue = info.GetValue(this).ToString();
                        GUI.Label(new Rect(x, y + i * 20, 100, 20), info.Name);
                        fieldvalue = GUI.TextField(new Rect(x + 100, y + i * 20, 100, 20), fieldvalue);
                        vl = fieldvalue;
                    }

                    info.SetValue(this, vl);
                }
            }
            catch (System.Exception ex)
            {
                //
            }
        }

        //menu add decision node
        private void menu_add_callback(object arg)
        {
            Type t = arg as Type;
            BNode node = Activator.CreateInstance(t) as BNode;

            this.AddChild(node);
            node.Parent = this;
            BTreeWin.sInstance.Repaint();
        }
        //menu switch node
        private void menu_switch_callback(object arg)
        {
            Type t = arg as Type;
            BNode node = Activator.CreateInstance(t) as BNode;
            node.Parent = this.Parent;
            foreach (BNode item in this.ChildList)
            {
                node.AddChild(item);
            }
            if (this.Parent != null)
            {
                this.Parent.ReplaceChild(this, node);
            }
            else if (BTreeWin.cur_tree.m_cRoot == this)
            {
                BTreeWin.cur_tree.m_cRoot = node;
            }
            BTreeWin.cur_node = node;
            BTreeWin.sInstance.Repaint();
        }

        //menu delete node
        private void menu_delete_node(object arg)
        {
            if (this.Parent != null)
            {
                this.Parent.RemoveChild(this);
            }
            this.Parent = null;
            BTreeWin.select = null;
            BTreeWin.cur_node = null;
            BTreeWin.sInstance.Repaint();
        }

        //render
        public virtual void Render(int x, ref int y)
        {
            Event evt = Event.current;
            if (BTreeWin.cur_node == this)
            {
                Texture2D texRed = new Texture2D(1, 1);
                texRed.SetPixel(0, 0, Color.blue);
                texRed.Apply();
                GUI.DrawTexture(new Rect(0, y, BTreeWin.sInstance.position.width, BTreeWin.NODE_HEIGHT), texRed);
            }

            Rect moveRect = new Rect(x, y, BTreeWin.sInstance.position.width - BTreeWin.GUI_WIDTH, 5);
            bool is_move_node = false;
            if (BTreeWin.select != null && moveRect.Contains(evt.mousePosition))
            {
                is_move_node = true;
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.green);
                tex.Apply();
                GUI.DrawTexture(new Rect(x, y, BTreeWin.sInstance.position.width, 2), tex);
                if (evt.button == 0 && evt.type == EventType.MouseUp)
                {
                    if (this != BTreeWin.select && this.Parent != null)
                    {
                        BTreeWin.select.Parent.RemoveChild(BTreeWin.select);
                        BTreeWin.select.Parent = this.Parent;
                        this.Parent.InsertChild(this, BTreeWin.select);
                    }
                    BTreeWin.select = null;
                    BTreeWin.sInstance.Repaint();
                }
            }

            Rect rect = new Rect(x, y, BTreeWin.sInstance.position.width - BTreeWin.GUI_WIDTH, BTreeWin.NODE_HEIGHT);
            if (!is_move_node && rect.Contains(evt.mousePosition))
            {
                if (BTreeWin.select != null)
                {
                    Texture2D texRed = new Texture2D(1, 1);
                    texRed.SetPixel(0, 0, Color.red);
                    texRed.Apply();
                    GUI.DrawTexture(new Rect(0, y, BTreeWin.sInstance.position.width, BTreeWin.NODE_HEIGHT), texRed);
                }
                if (evt.type == EventType.ContextClick)
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (Type item in BNodeFactory.sInstance.m_lstComposite)
                    {
                        menu.AddItem(new GUIContent("Create/Composite/" + item.Name), false, menu_add_callback, item);
                    }

                    foreach (Type item in BNodeFactory.sInstance.m_lstAction)
                    {
                        menu.AddItem(new GUIContent("Create/Action/" + item.Name), false, menu_add_callback, item);
                    }
                    foreach (Type item in BNodeFactory.sInstance.m_lstCondition)
                    {
                        menu.AddItem(new GUIContent("Create/Condition/" + item.Name), false, menu_add_callback, item);
                    }
                    foreach (Type item in BNodeFactory.sInstance.m_lstDecorator)
                    {
                        menu.AddItem(new GUIContent("Create/Decorator/" + item.Name), false, menu_add_callback, item);
                    }

                    foreach (Type item in BNodeFactory.sInstance.m_lstComposite)
                    {
                        menu.AddItem(new GUIContent("Switch/Composite/" + item.Name), false, menu_switch_callback, item);
                    }

                    menu.AddItem(new GUIContent("Delete"), false, menu_delete_node, "");
                    menu.ShowAsContext();
                }
                if (evt.button == 0 && evt.type == EventType.MouseDown && this != BTreeWin.cur_tree.m_cRoot)
                {
                    BTreeWin.select = this;
                    BTreeWin.cur_node = this;
                }
                if (evt.button == 0 && evt.type == EventType.MouseUp && BTreeWin.select != null)
                {
                    if (this != BTreeWin.select)
                    {
                        BTreeWin.select.Parent.RemoveChild(BTreeWin.select);
                        BTreeWin.select.Parent = this;
                        this.AddChild(BTreeWin.select);
                    }
                    BTreeWin.select = null;
                    BTreeWin.sInstance.Repaint();
                }
            }
            GUI.Label(new Rect(x, y, BTreeWin.sInstance.position.width, BTreeWin.NODE_HEIGHT), this.Name);

            /////////////////// line //////////////////////
            Vector3 pos1 = new Vector3(x + BTreeWin.NODE_WIDTH / 2, y + BTreeWin.NODE_HEIGHT, 0);
            Handles.color = Color.red;
            for (int i = 0; i < this.ChildList.Count; i++)
            {
                y = y + BTreeWin.NODE_HEIGHT;

                Vector3 pos2 = new Vector3(x + BTreeWin.NODE_WIDTH / 2, y + BTreeWin.NODE_HEIGHT / 2, 0);
                Vector3 pos3 = new Vector3(x + BTreeWin.NODE_WIDTH, y + BTreeWin.NODE_HEIGHT / 2, 0);
                this.ChildList[i].Render(x + BTreeWin.NODE_WIDTH, ref y);
                Handles.DrawPolyLine(new Vector3[] { pos1, pos2, pos3 });
            }
        }
#endif
        #endregion
    }
}