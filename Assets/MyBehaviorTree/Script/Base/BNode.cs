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
using System.Diagnostics;
//using Belong.BehaviorTree.Editor;
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
        [BHideField]
        public BTree Tree = null;
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

        private BAIBehaviorTree onwer = null;

        public BAIBehaviorTree Onwer
        {
            get
            {
                return onwer;
            }
        }


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
            this.Name = json["name"].ToString();
            JsonData arg = json["arg"];
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
                enode.Tree = this.Tree;
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
            FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
            Break: if (isBreak)
                {
                    continue;
                }

                if (!info.IsPublic)
                {
                    objs = info.GetCustomAttributes(typeof(BFieldAttribute), false);
                    if (objs.Length < 1)
                    {
                        isBreak = true;
                        goto Break;
                    }
                    for (int j = 0; j < objs.Length; j++)
                    {
                        BFieldAttribute bh = objs[j] as BFieldAttribute;
                        if (bh != null)
                        {
                            goto End;
                        }
                        else
                        {
                            isBreak = true;
                            goto Break;
                        }
                    }
                }
            End: json["arg"][info.Name] = info.GetValue(this).ToString();
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
            if (onwer == null)
            {
                onwer = AIManager.Instance.GetAI(Tree.AIGuid);
            }
            onwer.CurNode = this;
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

        #region 重载函数

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
        }

        public virtual void OnTriggerEnter(Collider collider)
        {

        }
        public virtual void OnTriggerExit(Collider collider)
        {

        }
        public virtual void OnTriggerStay(Collider collider)
        {

        }
        public virtual void OnCollisionEnter(Collision collision)
        {

        }
        public virtual void OnCollisionExit(Collision collision)
        {

        }
        public virtual void OnCollisionStay(Collision collision)
        {

        }
        public virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
        }
        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
        }
        public virtual void OnCollisionStay2D(Collision2D collision)
        {
        }
        public virtual void OnCollisionExit2D(Collision2D collision)
        {
        }
        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
        }
        public virtual void OnTriggerStay2D(Collider2D collider)
        {
        }
        public virtual void OnTriggerExit2D(Collider2D collider)
        {
        }


        #endregion

        #region 私有方法

        #endregion

    }
}