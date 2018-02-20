using System.Collections;
using UnityEngine;

public class PlayerMoveToPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayer;

    [Space(10)]
    public Transform desiredPosition;

    [Space(10)]
    public float minDistanceToPosition = 2f;

    [Header("DEBUG")]
    public float currentDistanceToPosition = 0f;

    private void Update()
    {
        StageEventAction();
    }

    public override void StageEventStart()
    {
        if (targetPlayer == null)
        {
            targetPlayer = ThirdPersonPlayerController.PlayerInstance;
        }

        if (desiredPosition == null)
        {
            desiredPosition = transform;
        }

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetPlayer != null && desiredPosition != null)
            {
                float distanceToPosition = Vector3.Distance(targetPlayer.position, desiredPosition.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= minDistanceToPosition)
                {
                    StageEventCompleted();
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayer.position, desiredPosition.position, Color.yellow);
                }
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
