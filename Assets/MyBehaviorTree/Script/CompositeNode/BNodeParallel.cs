/*
 * Belong
 * 2016-09-15
*/

namespace Belong.BehaviorTree
{
    /// <summary>
    /// 并行节点,直接点全部执行，其中一个失败则返回失败
    /// </summary>
    [BClass(ShowName = "并行节点")]
    public class BNodeParallel : BNodeComposite
    {
        /// <summary>
        /// 执行个数
        /// </summary>
        private int m_runingIndex;	
        /// <summary>
        /// 失败数量
        /// </summary>
        private int m_failure;

        public BNodeParallel()
            : base()
        {
            this.Name = "Parallel";
        }

        //onenter
        public override void OnEnter(object input)
        {
            this.m_runingIndex = 0;
            this.m_failure = 0;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override ActionResult Excute(object input)
        {
            if (this.m_runingIndex >= this.ChildList.Count)
            {
                if (this.m_failure > 0)
                {
                    return ActionResult.FAILURE;
                }
                return ActionResult.SUCCESS;
            }

            BNode node = this.ChildList[this.m_runingIndex];

            ActionResult res = node.RunNode(input);

            if (res == ActionResult.FAILURE)
            {
                this.m_failure++;
            }

            if (res != ActionResult.RUNNING)
            {
                this.m_runingIndex++;
            }


            return ActionResult.RUNNING;
        }
    }
}
