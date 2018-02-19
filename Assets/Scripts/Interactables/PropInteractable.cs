using System.Collections;
using UnityEngine;

public class PropInteractable : Interactable
{
    [Header("Custom Interaction Attributes")]
    public string propObjectName;

    public override void Interact(GameObject objectInteracting)
    {
        isBeingInteractedWith = true;

        Debug.Log("Interacting");
    }

    public override void StopInteraction()
    {
        isBeingInteractedWith = false;

        Debug.Log("No Longer Interacting");
    }
}
