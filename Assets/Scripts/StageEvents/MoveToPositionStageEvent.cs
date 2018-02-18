using System.Collections;
using UnityEngine;

public class MoveToPositionStageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetTransform;

    [Space(10)]
    public Transform desiredPosition;

    [Space(10)]
    public float minDistanceToPosition = 2f;

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
            if (targetTransform != null && desiredPosition != null)
            {
                float distanceToPosition = Vector3.Distance(targetTransform.position, desiredPosition.position);

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
