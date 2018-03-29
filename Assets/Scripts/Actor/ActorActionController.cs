using System.Collections;
using UnityEngine;

public class ActorActionController : MonoBehaviour
{
    private ActorController actorController;

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

    private void Start()
    {
        ActorActionControllerSetup();
    }

    private void ActorActionControllerSetup()
    {
        actorController = GetComponent<ActorController>();
    }

    public void ImportNewActorActions(ActorActionInfo actorActions, REPEAT_TYPE actorActionRepeatType)
    {
        currentActorEventIndex = 0;

        targetActorActions = actorActions.actorAction;

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

        currentActorAction.ActorActionSetup(this);

        currentActorAction.ActorActionStart();
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

    private void SceneEventFinished()
    {
        Debug.Log("Actor Received Scene Event Finished Pulse");
    }
}
