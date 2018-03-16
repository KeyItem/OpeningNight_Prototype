using System.Collections;
using UnityEngine;

public class PlayerMoveToPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayer;

    [Space(10)]
    public float minDistanceToPosition = 2f;

    [Header("DEBUG")]
    public float currentDistanceToPosition = 0f;

    public override void Update()
    {
        StageEventAction();

        base.Update();
    }

    public override void StageEventStart()
    {
        if (targetPlayer == null)
        {
            targetPlayer = ThirdPersonPlayerToolbox.Instance.transform;
        }

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetPlayer != null && stageEventTarget != null)
            {
                float distanceToPosition = Vector3.Distance(targetPlayer.position, stageEventTarget.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= minDistanceToPosition)
                {
                    isCountingEventTime = false;

                    StageEventCompleted();
                }
                else
                {
                    isCountingEventTime = true;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayer.position, stageEventTarget.position, Color.yellow);
                }
            }
        }
    }
}
