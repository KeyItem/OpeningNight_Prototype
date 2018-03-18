using System.Collections;
using UnityEngine;

public class ActorAction : MonoBehaviour
{
    [Header("Actor Action Base Attributes")]
    public ACTOR_ACTION_TYPE actorActionType;

    [Space(10)]
    public ActorMovementData actorMovementData;

    [HideInInspector]
    public ActorController actorPerformingAction;

    public virtual void ActorActionSetup(ActorController targetActorAction)
    {
        actorPerformingAction = targetActorAction;
    }

    public virtual void ActorActionStart()
    {
        
    }

    public virtual void ActorActionFinish()
    {
        actorPerformingAction.FinishedActorEvent();
    }
}

public enum ACTOR_ACTION_TYPE
{
    NONE,
    MOVE,
    INTERACT
}
