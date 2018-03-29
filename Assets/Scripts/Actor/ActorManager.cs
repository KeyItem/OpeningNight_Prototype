using System.Collections;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    private static ActorManager _instance;
    public static ActorManager Instance { get { return _instance; } }

    [Header("Scene Actors")]
    public ActorController[] sceneActors;

    private void Awake()
    {
        InitializeActorManager();
    }

    private void InitializeActorManager()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ReceiveActorActionsData(ActorActionData[] newActorActionData)
    {
        for (int i = 0; i < newActorActionData.Length; i++)
        {
            ActorController targetActorController = ReturnRequestedActorInScene(newActorActionData[i].targetActor);

            if (targetActorController != null)
            {
                targetActorController.ReceiveNewActorEvents(newActorActionData[i]);
            }
            else
            {
                Debug.LogError("Requested Actor Not In Scene :: " + newActorActionData[i].targetActor);
            }
        }
    }
    
    public ActorController ReturnRequestedActorInScene(ACTOR_NAME targetActorName)
    {
        if (targetActorName != ACTOR_NAME.NONE)
        {
            for (int i = 0; i < sceneActors.Length; i++)
            {
                if (sceneActors[i].actorName == targetActorName)
                {
                    return sceneActors[i];
                }
            }
        }

        return null;
    }
}

[System.Serializable]
public struct ActorActionData
{
    [Header("Actor Action Data Attributes")]
    public ACTOR_NAME targetActor;

    [Space(10)]
    public ActorActionInfo actorActions;

    [Space(10)]
    public REPEAT_TYPE actorActionRepeatType;

    [Space(20)]
    public ActorAnimationInfo actorAnimations;

    [Space(10)]
    public REPEAT_TYPE actorAnimationRepeatType;

    [Space(20)]
    public ActorMovementInfo actorMovements;

    [Space(10)]
    public REPEAT_TYPE actorMovementRepeatType;
}

[System.Serializable]
public struct ActorActionInfo
{
    public ActorAction[] actorAction;
}

[System.Serializable]
public struct ActorMovementInfo
{
    public ActorMovementData[] actorMovement;
}

[System.Serializable]
public class ActorMovementData
{
    public Transform[] actorMovePointTransform;

    [Space(10)]
    public float[] actorMovePointSpeeds;
}

[System.Serializable]
public struct ActorAnimationInfo
{
    public ActorAnimation[] actorAnimations;
}

[System.Serializable]
public struct ActorAnimation
{
    public AnimationClip targetActorAnimation;
}

public enum REPEAT_TYPE
{
    NONE,
    CYCLIC,
    REVERSE
}
