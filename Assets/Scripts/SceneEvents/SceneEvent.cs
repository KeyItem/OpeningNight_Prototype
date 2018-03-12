using System.Collections;
using UnityEngine;

public class SceneEvent : MonoBehaviour
{
    [Header("Scene Event Attributes")]
    public string sceneEventName;

    [Header("Scene Event Actor Attributes")]
    public ACTOR_NAME[] actorsInvolvedInScene;

    [Space(10)]
    public ActorMovementData[] actorMoveData;

    [Header("Scene Event Stage Event Attributes")]
    public StageEvent targetStageEvent;

    [Space(10)]
    public float stageEventActiveTime = 3f;

    [Space(10)]
    public bool stageEventHasActiveTime = false;

    [Header("Scene Event Stage Light Attributes")]
    public StageLightCommandData stageLightCommands;

    [Space(10)]
    public StageLightTargets[] stageLightTargetTransforms = new StageLightTargets[4];

    public virtual void SceneEventStart()
    {
        StageLightManager.Instance.ParseStageLightingCommandData(stageLightCommands, stageLightTargetTransforms);
    }

    public virtual void SceneEventCompleted()
    {
        if  (targetStageEvent == null)
        {
            StageLightManager.Instance.ClearCurrentStaticStageLights();
            StageLightManager.Instance.ClearCurrentEventStageLights();
        }
    }
}
