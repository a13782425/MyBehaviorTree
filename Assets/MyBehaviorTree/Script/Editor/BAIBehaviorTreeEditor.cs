using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

        void OnEnable()
        {
            _labelStyle = new GUIStyle();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BAIBehaviorTree tree = target as BAIBehaviorTree;
            Type treeType = tree.GetType();
            List<FieldInfo> fieldList = new List<FieldInfo>();
            FieldInfo[] fieldInfoBase = treeType.BaseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            FieldInfo[] fieldInfoCur = treeType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            fieldList.AddRange(fieldInfoBase);
            for (int i = 0; i < fieldInfoCur.Length; i++)
            {
                if (fieldInfoCur[i].DeclaringType != treeType.BaseType)
                {
                    fieldList.Add(fieldInfoCur[i]);
                }
            }


            for (int i = 0; i < fieldList.Count; i++)
            {
                FieldInfo info = fieldList[i];
                if (info.Name == "AITreeName")
                {
                    if (tree.AITreeText != null)
                    {

                        EditorLoad(tree.AITreeText);
                        EditorGUILayout.BeginHorizontal();
                        treeIndex = EditorGUILayout.Popup("AITreeName", treeIndex, treeNames);
                        EditorGUILayout.EndHorizontal();

                    }
                    else
                    {
                        treeIndex = -1;
                    }
                    continue;
                }

                if (info.IsPublic)
                {
                    object[] objs = info.GetCustomAttributes(typeof(HideInInspector), false);
                    if (objs.Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(info.Name));
                    }
                }
                else
                {
                    object[] objs = info.GetCustomAttributes(typeof(SerializeField), false);
                    if (objs.Length > 0)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(info.Name));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
            if (treeIndex != -1)
            {
                string str = treeNames[treeIndex];
                FieldInfo field = treeType.GetField("AITreeName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                field.SetValue(target, str);
            }
        }

        public void EditorLoad(TextAsset text)
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

    }
}
