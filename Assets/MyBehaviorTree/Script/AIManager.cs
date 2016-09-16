/*
 * Belong
 * 2016-09-16
*/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Belong.BehaviorTree
{
    public class AIManager : MonoBehaviour
    {
        private static AIManager _instance;

        private Dictionary<string, BAIBehaviorTree> m_dic = new Dictionary<string, BAIBehaviorTree>();

        public static AIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AIManager");
                    obj.AddComponent<AIManager>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        public void AddAI(BAIBehaviorTree tree)
        {
            if (m_dic.ContainsKey(tree.AIGuid))
            {
                m_dic.Add(tree.AIGuid, tree);
            }
        }
    }
}
