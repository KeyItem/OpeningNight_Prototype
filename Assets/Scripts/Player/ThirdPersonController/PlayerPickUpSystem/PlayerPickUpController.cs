using System.Collections;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    [Header("Player Pick Up Attributes")]
    public Transform targetAttachPointOnPlayer;

    [Space(10)]
    public GameObject currentHeldProp;

    [Space(10)]
    public Interactable currentInteractable;

    private Transform currentlyPickedUpObject;

    private Rigidbody currentObjectRigidbody;
    private Collider currentObjectCollider;

    [Space(10)]
    public bool canPlayerPickUpObject = true;

    [Space(10)]
    public bool isPlayerHoldingObject = false;

    public delegate void PlayerPickUp();
    public delegate void PlayerDrop();

    public static event PlayerPickUp OnPlayerPickup;
    public static event PlayerDrop OnPlayerDrop;

    public void PickUpObject(Interactable newObject)
    {
        if (OnPlayerPickup != null)
        {
            OnPlayerPickup();
        }

        currentInteractable = newObject;
        currentHeldProp = newObject.gameObject;
        currentlyPickedUpObject = newObject.transform;

        currentObjectRigidbody = newObject.GetComponent<Rigidbody>();
        currentObjectCollider = newObject.GetComponent<Collider>();

        currentObjectRigidbody.isKinematic = true;
        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), true);

        currentlyPickedUpObject.SetParent(targetAttachPointOnPlayer);

        currentlyPickedUpObject.localPosition = Vector3.zero;
        currentlyPickedUpObject.rotation = Quaternion.identity;

        PassOffInteractionAudio(INTERACTION_TYPE.PICKUP, currentInteractable);

        isPlayerHoldingObject = true;
    }

    public void DropObject()
    {
        if (OnPlayerDrop != null)
        {
            OnPlayerDrop();
        }

        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), false);

        currentInteractable.StopInteraction();

        currentlyPickedUpObject.SetParent(null);

        currentObjectRigidbody.isKinematic = false;

        PassOffInteractionAudio(INTERACTION_TYPE.DROP, currentInteractable);

        currentHeldProp = null;
        currentlyPickedUpObject = null;
        currentObjectRigidbody = null;
        currentObjectCollider = null;
        currentInteractable = null;

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

    private void PassOffInteractionAudio(INTERACTION_TYPE interactionType, Interactable interactionObject)
    {
        AudioClip newInteractionAudio = null;

        switch (interactionType)
        {
            case INTERACTION_TYPE.PICKUP:
                newInteractionAudio = interactionObject.interactionPickUpAudio;
                break;

            case INTERACTION_TYPE.DROP:
                newInteractionAudio = interactionObject.interactionDropAudio;
                break;

            case INTERACTION_TYPE.USE:
                newInteractionAudio = interactionObject.interactionAudio;
                break;

            default:
                break;
        }

        AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.SOUND_EFFECT, newInteractionAudio);
    }
}
