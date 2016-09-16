/*
 * Belong
 * 2016-09-16
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Belong.BehaviorTree
{

    public enum RunWhereEnum
    {
        Update,
        FixedUpdate,
        StartCoroutine,
        LateUpdate,
        Repeating
    }

    public class BAIBehaviorTree : MonoBehaviour
    {

        #region 成员变量

        #region  公共变量

        /// <summary>
        /// 当前AI的名字，没有作用
        /// </summary>
        public string AIName = string.Empty;

        /// <summary>
        /// AI配置文件
        /// </summary>
        public TextAsset AITreeText;

        /// <summary>
        /// 树的名字
        /// </summary>
        public string AITreeName = string.Empty;

        /// <summary>
        /// 运行间隔时间
        /// </summary>
        [SerializeField]
        private float Interval = 0.5f;
        /// <summary>
        /// 是否开始运行
        /// </summary>
        [SerializeField]
        private bool IsRun = true;
        /// <summary>
        /// 运行在哪个寄主上
        /// </summary>
        [SerializeField]
        protected RunWhereEnum RunEnum = RunWhereEnum.Update;

        /// <summary>
        /// 开始时间，仅对Repeating有效
        /// </summary>
        [SerializeField]
        private float StartTime = 0f;

        #endregion

        #region 私有变量

        [HideInInspector]
        public string AIGuid;
        /// <summary>
        /// 当前AI的树
        /// </summary>
        private BTree m_tree = null;
        /// <summary>
        /// 执行计时
        /// </summary>
        private float m_time = 0f;

        #endregion

        #region 继承变量

        /// <summary>
        /// 树执行时所传入的数据
        /// </summary>
        protected object MyData = null;

        #endregion

        #endregion

        void Awake()
        {
            Init();
            AIGuid = Guid.NewGuid().ToString();
            if (AITreeText == null)
            {
                Debug.LogError("没有树文件！");
                return;
            }
            if (string.IsNullOrEmpty(AITreeName))
            {
                AITreeName = BTreeMgr.Instance.Load(AITreeText.text);
            }
            else
            {
                BTreeMgr.Instance.Load(AITreeText.text);
            }
            m_tree = BTreeMgr.Instance.GetTree(AITreeName);
            if (string.IsNullOrEmpty(AIName))
            {
                AIName = AITreeName;
            }
            switch (RunEnum)
            {
                case RunWhereEnum.StartCoroutine:
                    StartCoroutine(RunTree());
                    break;
                case RunWhereEnum.Repeating:
                    InvokeRepeating("Excute", StartTime, Interval);
                    break;
                default:
                    break;
            }


        }

        IEnumerator RunTree()
        {
            while (IsRun)
            {
                Excute();
                yield return new WaitForSeconds(Interval);
            }
        }

        void Update()
        {
            if (RunEnum == RunWhereEnum.Update)
            {
                if (IsRun)
                {
                    m_time += Time.deltaTime * Time.timeScale;
                    if (m_time > Interval)
                    {
                        Excute();
                        m_time = 0f;
                    }
                }
            }
        }
        void FixedUpdate()
        {
            if (RunEnum == RunWhereEnum.FixedUpdate)
            {
                if (IsRun)
                {
                    m_time += Time.fixedDeltaTime * Time.timeScale;
                    if (m_time > Interval)
                    {
                        Excute();
                        m_time = 0f;
                    }
                }
            }
        }
        void LateUpdate()
        {
            if (RunEnum == RunWhereEnum.LateUpdate)
            {
                if (IsRun)
                {
                    m_time += Time.deltaTime * Time.timeScale;
                    if (m_time > Interval)
                    {
                        Excute();
                        m_time = 0f;
                    }
                }
            }
        }

        #region 继承方法

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void Init()
        {

        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 执行AI
        /// </summary>
        public void Excute()
        {
            m_tree.Run(MyData);
        }

        /// <summary>
        /// 停止AI
        /// </summary>
        public void StopAI()
        {
            IsRun = false;
            this.CancelInvoke();
        }

        /// <summary>
        /// 开始AI
        /// </summary>
        public void StartAI()
        {
            if (IsRun)
            {
                return;
            }
            IsRun = true;
            switch (RunEnum)
            {
                case RunWhereEnum.StartCoroutine:
                    StartCoroutine(RunTree());
                    break;
                case RunWhereEnum.Repeating:
                    InvokeRepeating("Excute", StartTime, Interval);
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
