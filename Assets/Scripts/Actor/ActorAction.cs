using System.Collections;
using UnityEngine;

public class ActorAction : MonoBehaviour
{
    [Header("Actor Action Base Attributes")]
    public ACTOR_ACTION_TYPE actorActionType;

    [HideInInspector]
    public ActorActionController actorPerformingAction;

    public virtual void ActorActionSetup(ActorActionController targetActorAction)
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
    INTERACT
}
