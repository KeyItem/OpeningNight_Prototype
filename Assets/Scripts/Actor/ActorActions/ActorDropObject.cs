using System.Collections;
using UnityEngine;

public class ActorDropObject : ActorAction
{
    [Header("Custom Actor Action Attributes")]
    private Rigidbody propRigidbody;

    public override void ActorActionStart()
    {
        if (actorPerformingAction != null)
        {
            Transform targetPropToPickup = actorPerformingAction.targetHoldPoint.GetChild(0);

            if (targetPropToPickup == null)
            {
                return;
            }

            propRigidbody = targetPropToPickup.GetComponent<Rigidbody>();

            Physics.IgnoreCollision(targetPropToPickup.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), false);

            targetPropToPickup.SetParent(null);

            propRigidbody.isKinematic = false;

            actorPerformingAction.isHoldingObject = false;

            ActorActionFinish();
        }
    }
}
