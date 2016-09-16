/*
 * Belong
 * 2016-09-15
*/
using UnityEngine;

namespace Belong.BehaviorTree
{
    /// <summary>
    /// 空的行为节点
    /// </summary>
    [BClass(ShowName = "空行为节点")]
    public class ActionNodeNothing : BNodeAction
    {
        public ActionNodeNothing()
            : base()
        {
            this.Name = "Nothing";
        }

        public override void OnEnter(object input)
        {
            Debug.LogError("测试空节点进入");
        }

        public override ActionResult Excute(object input)
        {
            return ActionResult.SUCCESS;
        }
        public override void OnExit(object input)
        {
            Debug.LogError("测试空节点退出");
        }

    }
}
