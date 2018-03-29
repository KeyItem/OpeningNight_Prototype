using System.Collections;
using UnityEngine;

public class PlayerStayInMovingArea_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform targetPlayerTransform;

    [Space(10)]
    public Transform targetZoneTransform;

    [Header("Waypoint Attributes")]
    public Transform[] targetWaypoints;

    [Space(10)]
    public Transform currentWaypoint;
    public Transform nextWaypoint;

    [Space(10)]
    public int currentWaypointIndex = 0;

    private float percentBetweenWaypoints = 0f;

    [Header("Movement Attributes")]
    public float moveSpeed = 1f;

    [Space(10)]
    [Range(0, 2)]
    public float moveEaseAmount;

    [Header("Waiting Attributes")]
    public float[] targetWaitTimes;

    private float nextWaitTime = 0f;

    [Header("Distance Attributes")]
    public float currentDistanceFromArea;

    [Space(10)]
    public float targetMaxDistanceFromArea = 2f;

    [Space(10)]
    public bool isTargetInArea = false;

    [Space(10)]
    public bool hasReachedFinalPosition = false;

    public override void Update()
    {
        ManageTargetInArea();
        StageEventAction();

        base.Update();
    }

    public override void StageEventStart()
    {
        currentWaypointIndex = 0;

        currentWaypoint = targetWaypoints[0];
        nextWaypoint = targetWaypoints[1];

        if (targetPlayerTransform == null)
        {
            targetPlayerTransform = ThirdPersonPlayerToolbox.Instance.transform;
        }

        if (targetZoneTransform == null)
        {
            targetZoneTransform = GetComponentInChildren<Transform>();
        }

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (hasReachedFinalPosition)
            {
                if (isTargetInArea)
                {
                    StageEventCompleted();
                }
            }
            else
            {
                ManageMovement();
            }
        }
    }

    private void ManageTargetInArea()
    {
        if (isStageEventActive)
        {
            float playerDistanceToPosition = Vector3.Distance(targetPlayerTransform.position, targetZoneTransform.position);

            if (playerDistanceToPosition <= targetMaxDistanceFromArea)
            {
                isTargetInArea = true;

                isStageEventCountingFailTime = false;
            }
            else
            {
                isTargetInArea = false;

                isStageEventCountingFailTime = true;
            }
        }
    }

    private bool TryMovingToNextWaypoint()
    {
        currentWaypointIndex++;

        int maxRange = currentWaypointIndex + 1;

        if (maxRange <= targetWaypoints.Length - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MoveToNextPoint()
    {
        percentBetweenWaypoints = 0;

        if (TryMovingToNextWaypoint())
        {
            currentWaypoint = targetWaypoints[currentWaypointIndex];
            nextWaypoint = targetWaypoints[currentWaypointIndex + 1];

            if (targetWaitTimes.Length > currentWaypointIndex && targetWaitTimes[currentWaypointIndex] > 0)
            {
                nextWaitTime = Time.time + targetWaitTimes[currentWaypointIndex];
            }
            else
            {
                nextWaitTime = 0f;
            }
        }
        else
        {
            FinishedRoute();
        }
    }

    private void ManageMovement()
    {
        if (!hasReachedFinalPosition)
        {
            if (Time.time > nextWaitTime)
            {
                float distanceToNextWaypoint = Vector3.Distance(currentWaypoint.position, nextWaypoint.position);

                percentBetweenWaypoints += Time.deltaTime * moveSpeed / distanceToNextWaypoint;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

                float easedPercent = EaseHelper.EaseValue(percentBetweenWaypoints, moveEaseAmount);

                Vector3 newTargetPosition = Vector3.Lerp(currentWaypoint.position, nextWaypoint.position, easedPercent);

                targetZoneTransform.position = newTargetPosition;

                if (percentBetweenWaypoints >= 1)
                {
                    MoveToNextPoint();
                }
            }           
        }    
    }

    private void FinishedRoute()
    {
        hasReachedFinalPosition = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (canShowDebug)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);

            Gizmos.DrawSphere(targetZoneTransform.position, targetMaxDistanceFromArea);
        }
    }
}
