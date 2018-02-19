using System.Collections;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    [Header("Player Pick Up Attributes")]
    public Transform targetAttachPointOnPlayer;

    [Space(10)]
    public GameObject currentInteractableObject;
    public Interactable currentInteractable;

    private Transform currentlyPickedUpObject;

    private Rigidbody currentObjectRigidbody;
    private Collider currentObjectCollider;

    [Space(10)]
    public bool canPlayerPickUpObject = true;

    [Space(10)]
    public bool isPlayerHoldingObject = false;

    public void PickUpObject(Interactable newObject)
    {
        currentInteractable = newObject;
        currentInteractableObject = newObject.gameObject;
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

        currentInteractable.StopInteraction();

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
