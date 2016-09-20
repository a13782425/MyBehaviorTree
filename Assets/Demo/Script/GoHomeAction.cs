using UnityEngine;
using System.Collections;
using Belong.BehaviorTree;

public class GoHomeAction : BNodeAction
{
    [BField(ShowName = "在家等待时间")]
    private float waitTime = 2f;

    private float nowTime;
    public override void OnEnter(object input)
    {
        nowTime = Time.time;
        Debug.LogError("Enter");
    }

    public override ActionResult Excute(object input)
    {
        if (Time.time - nowTime > waitTime)
        {
            (input as MyAttr).Health = 100;
            return ActionResult.SUCCESS;
        }
        return ActionResult.RUNNING;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        Debug.LogError("OnCollisionEnter");
        base.OnCollisionEnter(collision);
    }

    public override void OnCollisionStay(Collision collision)
    {
        Debug.LogError("OnCollisionStay");
        base.OnCollisionStay(collision);
    }
}
