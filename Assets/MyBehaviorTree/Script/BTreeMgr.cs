/*
 * Belong
 * 2016-09-15
*/

using LitJson;
using System.Collections.Generic;
using UnityEngine;

namespace Belong.BehaviorTree
{
    /// <summary>
    /// 行为树管理器
    /// </summary>
    public class BTreeMgr
    {
        public Dictionary<string, BTree> m_mapTree = new Dictionary<string, BTree>();

        private static BTreeMgr _instance;
        public static BTreeMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BTreeMgr();
                }
                return _instance;
            }
        }

        public BTreeMgr()
        {
            //
        }

        /// <summary>
        /// 加载树
        /// </summary>
        /// <param name="jsontxt"></param>
        /// <returns>返回第一棵树的名字</returns>
        public string Load(string jsontxt)
        {
            string treeName = string.Empty;
            JsonData json = JsonMapper.ToObject(jsontxt);
            json = json["trees"];
            int count = json.Count;
            for (int i = 0; i < count; i++)
            {

                BTree bt = new BTree();
                JsonData data = json[i];
                bt.ReadJson(data);
                if (i == 0)
                {
                    treeName = bt.m_treeName;
                }
                this.m_mapTree.Add(bt.m_treeName, bt);
            }
            return treeName;
        }

        public BTree GetTree(string name)
        {
            if (this.m_mapTree.ContainsKey(name))
                return this.m_mapTree[name];
            return null;
        }

        public List<BTree> GetTrees()
        {
            List<BTree> lst = new List<BTree>();
            foreach (BTree item in this.m_mapTree.Values)
                lst.Add(item);
            return lst;
        }

        public void Add(BTree tree)
        {
            if (this.m_mapTree.ContainsKey(tree.m_treeName))
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Error", "The tree named " + tree.m_treeName + " is already exist.", "ok");
#endif
                return;
            }
            this.m_mapTree.Add(tree.m_treeName, tree);
        }

        public void Remove(BTree tree)
        {
            if (tree == null) return;
            if (this.m_mapTree.ContainsKey(tree.m_treeName))
                this.m_mapTree.Remove(tree.m_treeName);
            else
                Debug.LogError("The tree id is not exist.");
            return;
        }
    }
}
