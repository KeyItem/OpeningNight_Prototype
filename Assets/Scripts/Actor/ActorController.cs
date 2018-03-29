using System.Collections;
using UnityEngine;

[RequireComponent(typeof (ActorActionController))]
[RequireComponent(typeof(ActorAnimationController))]
[RequireComponent(typeof(ActorNavigationController))]

public class ActorController : MonoBehaviour
{
    [Header("Actor Name Attributes")]
    public ACTOR_NAME actorName;

    [Header("Actor Action Info")]
    private ActorActionData currentActorActionData;

    [Header("Actor Action Controller")]
    public ActorActionController actorActionController;

    private ActorActionInfo currentActorActionInfo;

    [Header("Actor Animation Controller")]
    public ActorAnimationController actorAnimationController;

    private ActorAnimationInfo currentActorAnimationInfo;

    [Header("Actor Navigation Attributes")]
    public ActorNavigationController actorNavigationController;

    private ActorMovementInfo currentActorMovementInfo;

    private void Start()
    {
        ActorSetup();

        ActorEnable();
    }

    private void ActorSetup()
    {
        actorActionController = GetComponent<ActorActionController>();

        actorAnimationController = GetComponent<ActorAnimationController>();

        actorNavigationController = GetComponent<ActorNavigationController>();
    }

    private void ActorEnable()
    {
        SceneEventManager.OnSceneEventFinished += SceneEventFinished;
    }

    private void ActorDisable()
    {
        SceneEventManager.OnSceneEventFinished -= SceneEventFinished;
    }

    public void ReceiveNewActorEvents(ActorActionData newActorActionData)
    {
        currentActorActionData = newActorActionData;

        ParseActorActionData(newActorActionData);
    }

    private void ParseActorActionData(ActorActionData newActorActionData)
    {
        if (newActorActionData.actorActions.actorAction.Length > 0)
        {
            SendNewActionEvents(newActorActionData.actorActions, newActorActionData.actorActionRepeatType);
        }

        if (newActorActionData.actorAnimations.actorAnimations.Length > 0)
        {
            SendNewAnimationEvents(newActorActionData.actorAnimations, newActorActionData.actorAnimationRepeatType);
        }

        if (newActorActionData.actorMovements.actorMovement.Length > 0)
        {
            SendNewMovementEvents(newActorActionData.actorMovements, newActorActionData.actorMovementRepeatType);
        }
    }

    private void SendNewMovementEvents(ActorMovementInfo newActorMovementData, REPEAT_TYPE actionRepeatType)
    {
        actorNavigationController.ImportNewActorMovementInfo(newActorMovementData, actionRepeatType);
    }

    private void SendNewActionEvents(ActorActionInfo newActorActionInfo, REPEAT_TYPE actionRepeatType)
    {
        actorActionController.ImportNewActorActions(newActorActionInfo, actionRepeatType);
    }

    private void SendNewAnimationEvents(ActorAnimationInfo newActorAnimationInfo, REPEAT_TYPE actionRepeatType)
    {
        actorAnimationController.ImportNewActorAnimationInfo(newActorAnimationInfo, actionRepeatType);
    }

    private void FinishedAllActorEvents()
    {
        Debug.Log("All Actor Actions Finished");
    }

    private void SceneEventFinished()
    {
        Debug.Log("Actor Received Scene Event Finished Pulse");
    }
}
