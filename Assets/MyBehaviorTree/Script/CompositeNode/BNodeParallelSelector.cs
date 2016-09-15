
namespace Belong.BehaviorTree
{

    /// <summary>
    /// 并行选择节点，全部执行，其中一个成功则成功
    /// </summary>
    [BClass(ShowName = "并行选择节点")]
    public class BNodeParallelSelector : BNodeComposite
    {
        private int m_runingIndex;	//runing index
        private ActionResult m_result;
        public BNodeParallelSelector()
            : base()
        {
            this.Name = "ParallelSelector";
        }

        public override void OnEnter(object input)
        {
            this.m_runingIndex = 0;
            m_result = ActionResult.FAILURE;
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
                return m_result;
            }

            BNode node = this.ChildList[this.m_runingIndex];

            ActionResult res = node.RunNode(input);

            if (res == ActionResult.SUCCESS)
                m_result = res;

            if (res != ActionResult.RUNNING)
            {
                this.m_runingIndex++;
            }
            return ActionResult.RUNNING;
        }
    }
}
