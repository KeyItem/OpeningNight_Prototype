using System.Collections;
using UnityEngine;

public class SceneEvent : MonoBehaviour
{
    [Header("Scene Event Attributes")]
    public string sceneEventName;

    [Header("Scene Event Actor Attributes")]
    public ActorActionData[] actorActionsData;

    [Header("Scene Event Stage Event Attributes")]
    public StageEvent targetStageEvent;

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
