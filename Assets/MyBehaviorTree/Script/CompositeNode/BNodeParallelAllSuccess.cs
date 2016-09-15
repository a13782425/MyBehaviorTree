
namespace Belong.BehaviorTree
{
    /// <summary>
    /// 并行全部成功节点
    /// </summary>
    [BClass(ShowName = "并行全部成功")]
    public class BNodeParallelAllSuccess : BNodeParallel
    {
        /// <summary>
        /// 执行个数
        /// </summary>
        private int m_runningIndex;
        /// <summary>
        /// 结果
        /// </summary>
        private ActionResult m_result;	//result

        public BNodeParallelAllSuccess()
            : base()
        {
            this.Name = "ParallelAllSuccess";
        }

        public override void OnEnter(object input)
        {
            this.m_runningIndex = 0;
            this.m_result = ActionResult.SUCCESS;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override ActionResult Excute(object input)
        {
            if (this.m_runningIndex >= this.ChildList.Count)
            {
                return this.m_result;
            }

            ActionResult result = this.ChildList[this.m_runningIndex].RunNode(input);
            if (result == ActionResult.FAILURE)
            {
                this.m_result = ActionResult.FAILURE;
            }
            if (result != ActionResult.RUNNING)
                this.m_runningIndex++;

            return ActionResult.RUNNING;
        }
    }
}
