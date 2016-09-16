using UnityEngine;
using System.Collections;
using Belong.BehaviorTree;

public class CheckHealth : BNodeCondition
{
    [BField(ShowName = "回家血量")]
    public int GoHomeNum = 10;

    public override void OnEnter(object input)
    {
        Debug.LogError("判断");
    }

    public override ActionResult Excute(object input)
    {
        MyAttr my = input as MyAttr;
        if (my != null)
        {
            if (my.Health > GoHomeNum)
            {
                return ActionResult.FAILURE;
            }
            return ActionResult.SUCCESS;
        }
        return ActionResult.FAILURE;
    }

}
