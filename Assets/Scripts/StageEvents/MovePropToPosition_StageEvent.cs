using System.Collections;
using UnityEngine;

public class MovePropToPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetProp;

    [Space(10)]
    public Transform desiredPosition;

    [Space(10)]
    public float minDistanceToPosition = 2f;

    [Header("Debug Attributes")]
    public float currentDistanceToPosition = 0f;

    private void Update()
    {
        StageEventAction();
    }

    public override void StageEventStart()
    {
        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetProp != null && desiredPosition != null)
            {
                float distanceToPosition = Vector3.Distance(targetProp.position, desiredPosition.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= minDistanceToPosition)
                {
                    StageEventCompleted();
                }
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
