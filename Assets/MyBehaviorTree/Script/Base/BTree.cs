/*
 * Belong
 * 2016-09-15
*/

using LitJson;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Belong.BehaviorTree
{
    /// <summary>
    /// Behavior tree.
    /// </summary>
    public class BTree
    {
        public string m_treeName;   //描述
        public BNode m_root;	//root

        public string AIGuid;


        public BTree()
        {
        }

        //write json
        public void WriteJson(JsonData parent)
        {
            JsonData json = new JsonData();
            json["name"] = this.m_treeName;
            if (this.m_root != null)
            {
                json["node"] = new JsonData();
                json["node"].SetJsonType(JsonType.Object);
                json["node"] = this.m_root.WriteJson();
            }
            parent.Add(json);
        }

        public void ReadJson(string json)
        {
            ReadJson(JsonMapper.ToObject(json));
        }

        //read json
        public void ReadJson(JsonData json)
        {
            this.m_treeName = json["name"].ToString();
            this.m_root = null;
            if (json.Keys.Contains("node"))
            {
                string typename = json["node"]["type"].ToString();
                Type t = Type.GetType(typename);
                this.m_root = Activator.CreateInstance(t) as BNode;
                this.m_root.Tree = this;
                this.m_root.ReadJson(json["node"]);
            }
        }

        //set root node
        public void SetRoot(BNode node)
        {
            this.m_root = node;
        }

        //clear root node
        public void Clear()
        {
            this.m_root = null;
        }

        public void Run(object input)
        {
            this.m_root.RunNode(input);
        }

        public BTree Clone ()
        {
            BinaryFormatter bf = new BinaryFormatter();

            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);

            ms.Seek(0, SeekOrigin.Begin);

            BTree tree = bf.Deserialize(ms) as BTree;
            ms.Close();
            ms.Dispose();

            return tree;
            //return this.MemberwiseClone() as BTree;
        }

    }
}
