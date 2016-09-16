/*
 * Belong
 * 2016-09-15
*/

namespace Belong.BehaviorTree
{
    /// <summary>
    /// 迭代成功节点， 当运行成功节点数大于Num，返回成功，后面的不执行
    /// </summary>
    [BClass(ShowName = "迭代成功节点")]
    public class BNodeIteratorAllSuccess : BNodeIterator
    {

        public BNodeIteratorAllSuccess()
            : base()
        {
            this.Name = "IteratorAllSuccess";
        }
        public override ActionResult Excute(object input)
        {
            if (this.m_runningIndex >= this.ChildList.Count)
            {
                return ActionResult.FAILURE;
            }

            ActionResult res = this.ChildList[this.m_runningIndex].RunNode(input);
            if (res == ActionResult.SUCCESS)
            {
                this.m_successNum++;
            }
            if (this.m_successNum >= this.Num)
            {
                return ActionResult.SUCCESS;
            }
            if (res != ActionResult.RUNNING)
            {
                this.m_runningIndex++;
            }

            return ActionResult.RUNNING;
        }
    }
}
