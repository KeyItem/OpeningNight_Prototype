using System.Collections;
using UnityEngine;

public class PropMoveToPosition_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    [Header("Custom Stage Event Attributes")]
    public Transform targetProp;

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
        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetProp != null)
            {
                if (playerPickUpController.currentHeldProp == targetProp.gameObject)
                {
                    float distanceToPosition = Vector3.Distance(targetProp.position, stageEventTarget.position);

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
                        Debug.DrawLine(targetProp.position, transform.position, Color.yellow);
                    }
                }            
            }
        }
    }
}
