using System.Collections;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    [Header("Player Pick Up Attributes")]
    public Transform targetAttachPointOnPlayer;

    [Space(10)]
    public Interactable currentInteractableObject;

    private Transform currentlyPickedUpObject;

    private Rigidbody currentObjectRigidbody;
    private Collider currentObjectCollider;

    [Space(10)]
    public bool canPlayerPickUpObject = true;
    public bool isPlayerHoldingObject = false;

    public void PickUpObject(Interactable newObject)
    {
        currentInteractableObject = newObject;
        currentlyPickedUpObject = newObject.transform;

        currentObjectRigidbody = newObject.GetComponent<Rigidbody>();
        currentObjectCollider = newObject.GetComponent<Collider>();

        currentObjectRigidbody.isKinematic = true;
        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), true);

        currentlyPickedUpObject.SetParent(targetAttachPointOnPlayer);

        currentlyPickedUpObject.localPosition = Vector3.zero;
        currentlyPickedUpObject.rotation = Quaternion.identity;

        isPlayerHoldingObject = true;
    }

    public void DropObject()
    {
        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), false);

        currentInteractableObject.StopInteraction();

        currentlyPickedUpObject.SetParent(null);

        currentObjectRigidbody.isKinematic = false;

        currentInteractableObject = null;
        currentlyPickedUpObject = null;
        currentObjectRigidbody = null;
        currentObjectCollider = null;

        isPlayerHoldingObject = false;
    }

    public bool CanPlayerPickUpObject()
    {
        if (canPlayerPickUpObject)
        {
            if (!isPlayerHoldingObject)
            {
                return true;
            }
        }

        return false;
    }
}
