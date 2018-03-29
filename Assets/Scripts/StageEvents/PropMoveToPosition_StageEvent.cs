using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMoveToPosition_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    [Header("Custom Stage Event Attributes")]
    public Prop[] targetProps;

    [Space(10)]
    public float propMinDistanceToTargetPosition = 2f;

    public override void Update()
    {
        StageEventAction();

        base.Update();
    }

    public override void StageEventStart()
    {
        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetProps.Length > 0)
            {
                if (AreAllPropsInPosition())
                {
                    StageEventCompleted();
                }
            }
        }
    }

    private bool AreAllPropsInPosition()
    {
        for (int i = 0; i < targetProps.Length; i++)
        {
            float deltaDistance = Vector3.Distance(targetProps[i].targetProp.position, stageEventTarget.position);

            if (canShowDebug)
            {
                Debug.DrawLine(targetProps[i].targetProp.position, stageEventTarget.position, Color.yellow);
            }

            if (deltaDistance > propMinDistanceToTargetPosition)
            {
                return false;
            }
        }

        return true;
    }
}
