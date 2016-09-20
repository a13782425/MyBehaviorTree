/*
 * Belong
 * 2016-09-15
*/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Belong.BehaviorTree
{
    public class BNodeFactory
    {

//        private static BNodeFactory _instance;
//        public static BNodeFactory Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new BNodeFactory();
//                }
//                return _instance;
//            }
//        }

//        public BNodeFactory()
//        {
//        }

//#if UNITY_EDITOR
//        //get subclass
//        public static List<Type> GetSubClass(Type nodeType)
//        {
//            List<Type> lstType = new List<Type>();

//            string rootPath = Application.dataPath + "/";

//            List<string> files = getAllfiles(rootPath);
//            foreach (string name in files)
//            {
//                string class_name = name.Split('.')[0];
//                Type tt = Type.GetType("Game.AIBehaviorTree." + class_name);
//                if (tt != null)
//                {
//                    if (tt.IsSubclassOf(nodeType))
//                    {
//                        lstType.Add(tt);
//                    }
//                }
//            }
//            return lstType;
//        }
//        //get all files in dir and subdir
//        private static List<string> getAllfiles(string dir)
//        {
//            List<string> lst = new List<string>();
//            DirectoryInfo dirInfo = new DirectoryInfo(dir);
//            foreach (FileInfo file in dirInfo.GetFiles("*.cs"))
//            {
//                lst.Add(file.Name);
//            }
//            string[] dirs = Directory.GetDirectories(dir);
//            for (int i = 0; i < dirs.Length; i++)
//            {
//                string path = dirs[i];
//                {
//                    lst.AddRange(getAllfiles(path));
//                }
//            }
//            return lst;
//        }
//#endif

        //public BNode Create(int index)
        //{
        //    if (this.m_lstComposite.Count > index)
        //    {
        //        Type t = this.m_lstComposite[index];
        //        BNode node = Activator.CreateInstance(t) as BNode;
        //        return node;
        //    }
        //    Debug.LogError("The type index is none : " + index);
        //    return null;
        //}

        //public string[] GetNodeLst()
        //{
        //    string[] str = new string[this.m_lstComposite.Count];
        //    for (int i = 0; i < this.m_lstComposite.Count; i++)
        //    {
        //        Type item = this.m_lstComposite[i];
        //        str[i] = item.Name;
        //    }
        //    return str;
        //}
    }
}
