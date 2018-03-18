using System.Collections;
using UnityEngine;

public class ActorPickUpObject : ActorAction
{
    [Header("Custom Actor Action Attributes")]
    public Transform targetPropToPickup;

    private Rigidbody propRigidbody;

    public override void ActorActionStart()
    {
        if (actorPerformingAction != null)
        {
            if (targetPropToPickup != null)
            {
                propRigidbody = targetPropToPickup.GetComponent<Rigidbody>();

                propRigidbody.isKinematic = true;
                Physics.IgnoreCollision(targetPropToPickup.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);

                targetPropToPickup.SetParent(actorPerformingAction.targetHoldPoint);

                targetPropToPickup.localPosition = Vector3.zero;
                targetPropToPickup.rotation = Quaternion.identity;

                actorPerformingAction.isHoldingObject = true;

                ActorActionFinish();
            }
        }
    }
}
