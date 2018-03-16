using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Base Interactable Object Attributes")]
    public string interactableObjectName;

    [Space(10)]
    public INTERACTABLE_OBJECT_TYPE interactableObjectType;

    [Space(10)]
    public bool isPlaced = true;

    [Space(10)]
    public bool canBeInteractedWith = true;

    [Space(10)]
    public bool isBeingInteractedWith = false;

    [Header("Base Interactable Audio Attributes")]
    public AudioClip interactionAudio;
    public AudioClip interactionPickUpAudio;
    public AudioClip interactionDropAudio;

    public virtual void Interact(GameObject playerInteracting)
    {

    }

    public virtual void InteractWithProp(GameObject objectToInteractWith)
    {

    }

    public virtual void StopInteraction()
    {

    }
}
