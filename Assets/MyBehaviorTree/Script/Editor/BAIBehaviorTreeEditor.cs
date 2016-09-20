/*
 * Belong
 * 2016-09-18
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Belong.BehaviorTree.Editor
{

    [CustomEditor(typeof(BAIBehaviorTree), true)]
    public class BAIBehaviorTreeEditor : UnityEditor.Editor
    {

        private BTree cur_tree;
        private string[] treeNames;
        private int treeIndex = -1;
        private GUIStyle _labelStyle;

        private List<FieldInfo> fieldInfoList = new List<FieldInfo>();

        void OnEnable()
        {
            _labelStyle = new GUIStyle();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            fieldInfoList.Clear();
            treeNames = null;
            BAIBehaviorTree tree = target as BAIBehaviorTree;
            Type treeType = target.GetType();

            GetField(treeType);
            for (int i = 0; i < fieldInfoList.Count; i++)
            {
                FieldInfo info = fieldInfoList[i];
                if (info.Name == "AITreeName")
                {
                    if (tree.AITreeText != null)
                    {

                        EditorLoad(tree.AITreeText);
                        if (treeNames != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            treeIndex = EditorGUILayout.Popup("AI Tree Name", treeIndex, treeNames);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        treeIndex = -1;
                    }
                    continue;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty(info.Name));
            }

            serializedObject.ApplyModifiedProperties();
            if (treeIndex != -1)
            {
                string str = treeNames[treeIndex];
                FieldInfo field = treeType.GetField("AITreeName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                field.SetValue(target, str);
            }
        }


        private void GetField(Type type)
        {
            if (type.BaseType != typeof(MonoBehaviour))
            {
                GetField(type.BaseType);
            }
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo info = fieldInfos[i];
                if (info.DeclaringType == info.ReflectedType)
                {
                    if (info.IsPublic)
                    {
                        object[] objs = info.GetCustomAttributes(typeof(HideInInspector), false);
                        if (objs.Length > 0)
                        {
                            continue;
                        }
                        else
                        {
                            fieldInfoList.Add(info);
                        }
                    }
                    else
                    {
                        object[] objs = info.GetCustomAttributes(typeof(SerializeField), false);
                        if (objs.Length > 0)
                        {
                            fieldInfoList.Add(info);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }


        public void EditorLoad(TextAsset text)
        {
            try
            {
                BTreeMgr.Instance.m_mapTree.Clear();
                string txt = text.text;
                BTreeMgr.Instance.Load(txt);
                treeNames = new string[BTreeMgr.Instance.GetTrees().Count];
                for (int i = 0; i < BTreeMgr.Instance.GetTrees().Count; i++)
                {
                    treeNames[i] = BTreeMgr.Instance.GetTrees()[i].m_treeName;
                }
            }
            catch (Exception)
            {
                treeNames = null;
                Debug.LogError("请使用BTree/Create创建树文件，在对其赋值");
            }
         
        }

    }
}
