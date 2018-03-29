using System.Collections;
using UnityEngine;

public class PropStayInArea_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    [Header("Custom Stage Event Attributes")]
    public Prop[] targetProps;

    [Space(10)]
    public float targetInAreaWaitTime = 1f;
    private float currentAreaWaitTime;

    [Space(10)]
    public float targetMinDistance = 2f;

    [Space(10)]
    public bool propMustBeHeld = false;

    [Space(10)]
    public bool areAllTargetsInArea = false;

    public override void Update()
    {
        StageEventAction();

        ManageTargetInArea();

        base.Update();
    }

    public override void StageEventStart()
    {
        currentAreaWaitTime = targetInAreaWaitTime;

        if (targetProps == null)
        {
            Debug.LogError("Prop is not assigned :: " + this);
        }

        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (targetProps != null && stageEventTarget != null)
            {
                if (AreAllPropsInArea())
                {
                    if (propMustBeHeld)
                    {
                        if (IsPropHeldInPosition())
                        {
                            areAllTargetsInArea = true;
                        }
                        else
                        {
                            areAllTargetsInArea = false;
                        }
                    }
                    else
                    {
                        areAllTargetsInArea = true;
                    }
                }
                else
                {
                    areAllTargetsInArea = false;
                }
            }
        }
    }

    private void ManageTargetInArea()
    {
        if (isStageEventActive)
        {
            if (areAllTargetsInArea)
            {
                currentAreaWaitTime -= Time.deltaTime;

                if (currentAreaWaitTime <= 0)
                {
                    StageEventCompleted();
                }
            }
            else
            {
                currentAreaWaitTime = targetInAreaWaitTime;
            }
        }
    }

    private bool AreAllPropsInArea()
    {
        for (int i = 0; i < targetProps.Length; i++)
        {
            float deltaDistance = Vector3.Distance(targetProps[i].targetProp.position, stageEventTarget.position);

            if (canShowDebug)
            {
                Debug.DrawLine(targetProps[i].targetProp.position, stageEventTarget.position, Color.yellow);
            }

            if (deltaDistance >= targetMinDistance)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsPropHeldInPosition()
    {
        for (int i = 0; i < targetProps.Length; i++)
        {
            if (targetProps[i].targetProp == playerPickUpController.currentHeldProp)
            {
                return true;
            }
        }

        return false;
    }
}
