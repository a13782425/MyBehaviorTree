
namespace Belong.BehaviorTree
{
    /// <summary>
    /// 选择节点,遇到成功返回成功,后续的不执行
    /// </summary>
    [BClass(ShowName = "选择节点")]
    public class BNodeSelector : BNodeComposite
    {
        private int m_runingIndex;	//runing index

        public BNodeSelector()
            : base()
        {
            this.Name = "Selector";
        }

        public override void OnEnter(object input)
        {
            this.m_runingIndex = 0;
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
                return ActionResult.FAILURE;
            }

            BNode node = this.ChildList[this.m_runingIndex];

            ActionResult res = node.RunNode(input);

            if (res == ActionResult.SUCCESS)
                return ActionResult.SUCCESS;

            if (res == ActionResult.FAILURE)
            {
                this.m_runingIndex++;
            }
            return ActionResult.RUNNING;
        }
    }
}
