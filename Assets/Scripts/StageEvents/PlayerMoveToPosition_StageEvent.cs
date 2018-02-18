﻿using System.Collections;
using UnityEngine;

public class PlayerMoveToPosition_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayer;

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
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;

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
            }
        }
    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }

}
