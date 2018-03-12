using System.Collections;
using UnityEngine;

public class PlayerDropProp_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    public override void StageEventStart()
    {
        playerPickUpController = ThirdPersonPlayerToolbox.Instance.ThirdPersonPickUpController;

        PlayerPickUpController.OnPlayerDrop += StageEventCompleted;

        base.StageEventStart();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            StageEventCompleted();
        }
    }

    public override void StageEventCompleted()
    {
        PlayerPickUpController.OnPlayerDrop -= StageEventCompleted;

        base.StageEventCompleted();
    }
}
