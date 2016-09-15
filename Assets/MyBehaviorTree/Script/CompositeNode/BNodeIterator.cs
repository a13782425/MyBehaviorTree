
namespace Belong.BehaviorTree
{
    /// <summary>
    /// 迭代节点，遇到失败且运行节点数量小于NUM返回失败，成功个数大于Num返回成功
    /// </summary>
    [BClass(ShowName = "迭代节点")]
    public class BNodeIterator : BNodeComposite
    {
        /// <summary>
        /// 成功个数
        /// </summary>
        [BField(ShowName = "成功个数")]
        public int Num;

        protected int m_runningIndex;
        /// <summary>
        /// 当前运行成功的个数
        /// </summary>
        protected int m_successNum;

        public BNodeIterator()
            : base()
        {
            this.Name = "Iterator";
        }

        public override void OnEnter(object input)
        {
            this.m_runningIndex = 0;
            this.m_successNum = 0;
        }

        public override ActionResult Excute(object input)
        {
            if (this.m_runningIndex >= this.ChildList.Count)
            {
                return ActionResult.FAILURE;
            }

            ActionResult res = this.ChildList[this.m_runningIndex].RunNode(input);

            if (res == ActionResult.FAILURE)
                return ActionResult.FAILURE;

            if (res == ActionResult.SUCCESS)
            {
                this.m_runningIndex++;
                this.m_successNum++;
            }

            if (this.m_successNum >= this.Num)
                return ActionResult.SUCCESS;

            return ActionResult.RUNNING;
        }
    }
}
