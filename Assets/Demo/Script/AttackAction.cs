using UnityEngine;
using System.Collections;
using Belong.BehaviorTree;

public class AttackAction : BNodeAction
{

    [BField(ShowName = "伤害")]
    private int Hit = 10;

    public override void OnEnter(object input)
    {
        Debug.LogError(Onwer.name);
        //Debug.LogError("进入伤害");
    }

    public override ActionResult Excute(object input)
    {
        MyAttr attr = input as MyAttr;
        if (attr != null)
        {
            attr.Health -= Hit;

            return ActionResult.SUCCESS;
        }
        return ActionResult.FAILURE;

    }

}
