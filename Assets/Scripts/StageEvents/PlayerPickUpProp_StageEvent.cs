using System.Collections;
using UnityEngine;

public class PlayerPickUpProp_StageEvent : StageEvent
{
    [Header("Custom Stage Event Attributes")]
    public Transform pickedUpByTransform;

    private Transform targetTransform;

    [Space(10)]
    public bool isPickedUp = false;

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
            if (isPickedUp)
            {

            }
        }
    }

    public void GetPickedUp()
    {
        if (!isPickedUp)
        {

        }
    }

    private void PickedUp()
    {

    }

    public override void StageEventCompleted()
    {
        base.StageEventCompleted();
    }
}
