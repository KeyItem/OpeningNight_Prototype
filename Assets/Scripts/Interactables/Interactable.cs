using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Base Interactable Object Attributes")]
    public INTERACTABLE_OBJECT_TYPE interactableObjectType;

    [Space(10)]
    public bool isBeingInteractedWith = false;

    public virtual void Interact(GameObject objectInteracting)
    {

    }

    public virtual void StopInteraction()
    {

    }
}
