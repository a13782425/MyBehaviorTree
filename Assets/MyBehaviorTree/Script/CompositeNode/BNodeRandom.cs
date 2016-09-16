/*
 * Belong
 * 2016-09-15
*/

using UnityEngine;

namespace Belong.BehaviorTree
{
    /// <summary>
    /// 随机节点
    /// </summary>
    [BClass(ShowName = "随机节点")]
    public class BNodeRandom : BNodeComposite
    {
        private int m_runningIndex;
        public BNodeRandom()
            : base()
        {
            this.Name = "Random";
        }

        public override void OnEnter(object input)
        {
            this.m_runningIndex = Random.Range(0, this.ChildList.Count);
            base.OnEnter(input);
        }

        //excute
        public override ActionResult Excute(object input)
        {
            return this.ChildList[this.m_runningIndex].RunNode(input);
        }
    }
}
