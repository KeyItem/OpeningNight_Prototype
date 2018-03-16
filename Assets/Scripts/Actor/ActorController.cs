using System.Collections;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    [Header("Actor Name Attributes")]
    public ACTOR_NAME actorName;

    [Header("Actor Animation Controller")]
    public ActorAnimationController actorAnimation;

    [Header("Actor Navigation Attributes")]
    public ActorNavigationController actorNavigation;

    private void Start()
    {
        ActorSetup();
    }

    private void Update()
    {
       
    }

    private void ActorSetup()
    {
        actorAnimation = GetComponent<ActorAnimationController>();
        actorNavigation = GetComponent<ActorNavigationController>();
    }

    public void ReceiveNewMovementCommand(ActorMovementData newActorMovementData)
    {
        actorNavigation.ReceiveNewActorMovementData(newActorMovementData);
    }
}

public enum ACTOR_COMMAND_TYPE
{
    NONE,
    MOVE,
    INTERACT
}
