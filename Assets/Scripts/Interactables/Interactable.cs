using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactable Object Attributes")]
    public INTERACTABLE_OBJECT_TYPE interactableObjectType;

    public virtual void Interact(GameObject objectInteracting)
    {

    }
}
