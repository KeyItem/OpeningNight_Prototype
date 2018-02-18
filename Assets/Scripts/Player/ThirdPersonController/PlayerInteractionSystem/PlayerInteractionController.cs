using System.Collections;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Player Interaction Attributes")]
    public GameObject[] interactableObjectsInRange;

    private PlayerInteractionAttributes playerInteractionAttributes;

    [Space(10)]
    public LayerMask interactableLayerMask;

    [Space(10)]
    private RaycastHit interactRayHit;

    [Space(10)]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializePlayerInteraction();
    }

    private void Update()
    {
        CheckForInteractableObjects();
    }

    private void InitializePlayerInteraction()
    {
        playerInteractionAttributes = GetComponent<ThirdPersonPlayerController>().playerInteractionAttributes;
    }

    public void AttemptPlayerInteraction()
    {
        if (interactableObjectsInRange.Length > 0)
        {
            Interactable newInteractableObject = ReturnClosestInteractableObject();

            if (CheckForInteractAngle(newInteractableObject))
            {
                float distanceFromPlayer = Vector3.Distance(transform.position, newInteractableObject.transform.position);

                if (CheckPathToInteractableObject(newInteractableObject, distanceFromPlayer))
                {
                    newInteractableObject.Interact(gameObject);
                }
                else
                {
                    Debug.Log("No Path to Object Interaction :: " + newInteractableObject);
                }
            }
        }
    }

    private Interactable ReturnClosestInteractableObject()
    {
        Interactable closestInteractableObject = null;

        float closestInteractableObjectDistance = float.MaxValue;

        for (int i = 0; i < interactableObjectsInRange.Length; i++)
        {
            Interactable newInteractable = interactableObjectsInRange[i].GetComponent<Interactable>();

            float interactableObjectDistance = Vector3.Distance(transform.position, newInteractable.transform.position);

            if (interactableObjectDistance < closestInteractableObjectDistance)
            {
                closestInteractableObjectDistance = interactableObjectDistance;

                closestInteractableObject = newInteractable;
            }
        }

        return closestInteractableObject;
    }

    private bool CheckPathToInteractableObject(Interactable interactableObject, float playerDistanceFromInteractable)
    {
        float verticalDistanceBetweenInteraction = Mathf.Abs(interactableObject.transform.position.y - transform.position.y);

        if (verticalDistanceBetweenInteraction > playerInteractionAttributes.playerInteractionMaxVerticalHeight) //Check if Cover is on similar Y coordinates
        {
            Debug.Log("Object is too high :: " + verticalDistanceBetweenInteraction);

            return false;
        }

        Vector3 interceptVector = (interactableObject.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, interceptVector, out interactRayHit, playerDistanceFromInteractable))
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * playerDistanceFromInteractable, Color.red, 1f);
            }

            if (interactRayHit.collider.gameObject != interactableObject.gameObject)
            {
                Debug.Log(interactRayHit.collider.gameObject.name);

                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * playerDistanceFromInteractable, Color.green, 1f);
            }

            return true;
        }
    }

    private bool CheckForInteractAngle(Interactable interactableObject)
    {
        if (interactableObject.interactableObjectType == INTERACTABLE_OBJECT_TYPE.ACTOR)
        {
            Vector3 interceptVector = interactableObject.transform.position - transform.position;

            float vectorAngle = Vector3.Angle(interceptVector, transform.forward);

            if (vectorAngle > playerInteractionAttributes.playerInteractionActorMaxAngle)
            {
                Debug.Log(vectorAngle);

                return false;
            }
        }
        else if (interactableObject.interactableObjectType == INTERACTABLE_OBJECT_TYPE.PHYSICS)
        {
            Vector3 interceptVector = interactableObject.transform.position - transform.position;

            float vectorAngle = Vector3.Angle(interceptVector, transform.forward);

            if (vectorAngle > playerInteractionAttributes.playerInteractionPhysicsMaxAngle)
            {
                return false;
            }
        }

        return true;
    }

    private void CheckForInteractableObjects()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, playerInteractionAttributes.playerInteractionMaxRadius, interactableLayerMask);
        interactableObjectsInRange = new GameObject[objectsInRange.Length];

        for (int i = 0; i < objectsInRange.Length; i++)
        {
            interactableObjectsInRange[i] = objectsInRange[i].gameObject;
        }
    }
}
