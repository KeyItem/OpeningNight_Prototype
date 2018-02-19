using System.Collections;
using UnityEngine;

public class PropInteractable : Interactable
{
    [Header("Custom Interaction Attributes")]
    public string propObjectName;

    public override void Interact(GameObject playerInteracting)
    {
        isBeingInteractedWith = true;
    }

    public override void StopInteraction()
    {
        isBeingInteractedWith = false;
    }
}
