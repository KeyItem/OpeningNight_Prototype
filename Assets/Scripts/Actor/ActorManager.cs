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
                targetActorController.ReceiveNewActorActions(newActorActionData[i].actorActions);
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
}

[System.Serializable]
public struct ActorActionInfo
{
    public ActorAction[] actions;
}
