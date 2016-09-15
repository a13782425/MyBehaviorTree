
namespace Belong.BehaviorTree
{
    /// <summary>
    /// 顺序节点,其中一个失败就返回失败，后续不执行
    /// </summary>
    [BClass(ShowName = "顺序节点")]
    public class BNodeSequence : BNodeComposite
    {
        private int m_runingIndex;

        public BNodeSequence()
            : base()
        {
            this.Name = "Sequence";
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
                return ActionResult.SUCCESS;
            }

            BNode node = this.ChildList[this.m_runingIndex];

            ActionResult res = node.RunNode(input);

            if (res == ActionResult.FAILURE)
                return ActionResult.FAILURE;

            if (res == ActionResult.SUCCESS)
            {
                this.m_runingIndex++;
            }

            return ActionResult.RUNNING;
        }
    }
}
