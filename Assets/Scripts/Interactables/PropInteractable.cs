using System.Collections;
using UnityEngine;

public class PropInteractable : Interactable
{
    public override void Interact(GameObject playerInteracting)
    {
        if (isPlaced)
        {
            if (canBeInteractedWith)
            {
                isBeingInteractedWith = true;
            }
        }
    }

    public override void StopInteraction()
    {
        isBeingInteractedWith = false;
    }
}
