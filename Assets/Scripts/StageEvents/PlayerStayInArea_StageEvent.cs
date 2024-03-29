﻿using System.Collections;
using UnityEngine;

public class PlayerStayInArea_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayerTransform;

    [Space(10)]
    public float targetInAreaWaitTime = 1f;

    [Space(10)]
    public float targetMinDistance = 2f;

    [Space(10)]
    public bool isTargetInArea = false;

    [Header("DEBUG")]
    public float currentDistanceToPosition = 0f;

    [Space(10)]
    public float targetWaitTime = 1f;

    private void Update()
    {
        StageEventAction();

        ManageTargetInArea();
    }

    public override void StageEventStart()
    {
        targetWaitTime = targetInAreaWaitTime;

        if (targetPlayerTransform == null)
        {
            targetPlayerTransform = ThirdPersonPlayerToolbox.Instance.transform;
        }

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetPlayerTransform != null && stageEventTarget != null)
            {
                float distanceToPosition = Vector3.Distance(targetPlayerTransform.position, stageEventTarget.position);

                currentDistanceToPosition = distanceToPosition;

                if (distanceToPosition <= targetMinDistance)
                {
                    isTargetInArea = true;
                }
                else
                {
                    isTargetInArea = false;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayerTransform.position, stageEventTarget.position, Color.yellow);
                }
            }
        }
    }

    private void ManageTargetInArea()
    {
        if (isStageEventActive)
        {
            if (isTargetInArea)
            {
                targetWaitTime -= Time.deltaTime;

                if (targetWaitTime <= 0)
                {
                    StageEventCompleted();
                }
            }
            else
            {
                targetWaitTime = targetInAreaWaitTime;
            }
        }
    }
}
