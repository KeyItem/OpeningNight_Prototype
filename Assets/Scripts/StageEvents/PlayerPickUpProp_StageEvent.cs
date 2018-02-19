using System.Collections;
using UnityEngine;

public class PlayerPickUpProp_StageEvent : StageEvent
{
    private PlayerPickUpController playerPickUpController;

    private void Update()
    {
        StageEventAction();
    }

    public override void StageEventStart()
    {
        base.StageEventStart();

        playerPickUpController = ThirdPersonPlayerController.PlayerInstance.GetComponent<PlayerPickUpController>();
    }

    public override void StageEventAction()
    {
        if (isStageEventActive)
        {
            if (playerPickUpController.isPlayerHoldingObject)
            {
                if (playerPickUpController.currentInteractableObject == gameObject)
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
