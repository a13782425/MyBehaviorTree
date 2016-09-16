using UnityEngine;
using System.Collections;
using Belong.BehaviorTree;
public class MyTree : BAIBehaviorTree
{
    public Transform Test;

    private MyAttr input;

    // Use this for initialization

    protected override void Init()
    {
        base.Init();
        MyData = new MyAttr();
    }

    // Update is called once per frame

    void OnGUI()
    {
        if (GUILayout.Button("StartAI"))
        {
            StartAI();
        }
        if (GUILayout.Button("StopAI"))
        {
            StopAI();
        }
    }
}
