using System.Collections;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    [Header("Actor Name Attributes")]
    public ACTOR_NAME actorName;

    [Header("Actor Actions")]
    public ActorAction[] targetActorActions;

    [Space(10)]
    public ActorAction currentActorAction;

    [Space(10)]
    public int currentActorEventIndex;

    [Header("Actor Interaction Attributes")]
    public Transform targetHoldPoint;

    [Space(10)]
    public bool isHoldingObject = false;

    [Header("Actor Animation Controller")]
    private ActorAnimationController actorAnimation;

    [Header("Actor Navigation Attributes")]
    private ActorNavigationController actorNavigation;

    private void Start()
    {
        ActorSetup();
    }

    private void ActorSetup()
    {
        actorAnimation = GetComponent<ActorAnimationController>();

        actorNavigation = GetComponent<ActorNavigationController>();
        actorNavigation.targetActorController = this;
    }

    public void ReceiveNewActorActions(ActorActionInfo actorActions)
    {
        targetActorActions = actorActions.actions;

        currentActorEventIndex = 0;

        PlayActorEvent(targetActorActions[0]);
    }

    private void PlayNextActorEvent()
    {
        if (CanMoveToNextActorEvent())
        {
            PlayActorEvent(targetActorActions[currentActorEventIndex]);
        }
        else
        {
            FinishedAllActorEvents();
        }
    }

    private void PlayActorEvent(ActorAction actorAction)
    {
        currentActorAction = actorAction;

        switch (currentActorAction.actorActionType)
        {
            case ACTOR_ACTION_TYPE.MOVE:
                actorNavigation.ReceiveNewActorMovementData(currentActorAction.actorMovementData);
                break;

            case ACTOR_ACTION_TYPE.INTERACT:
                break;
        }

        currentActorAction.ActorActionSetup(this);

        currentActorAction.ActorActionStart();
    }

    private void SendNewMovementAction(ActorMovementData newActorMovementData)
    {
        actorNavigation.ReceiveNewActorMovementData(newActorMovementData);
    }

    public void FinishedActorEvent()
    {
        currentActorAction = null;

        PlayNextActorEvent();
    }

    private void FinishedAllActorEvents()
    {

    }

    private bool CanMoveToNextActorEvent()
    {
        int actorEventIndex = currentActorEventIndex;

        if (++actorEventIndex <= targetActorActions.Length - 1)
        {
            currentActorEventIndex++;

            return true;
        }

        return false;
    }
}
