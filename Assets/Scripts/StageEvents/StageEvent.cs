using System.Collections;
using UnityEngine;

[System.Serializable]
public class StageEvent : MonoBehaviour
{
    [Header("Base Stage Event Attributes")]
    public string stageEventName;

    [Space(10)]
    public Transform stageEventTarget;

    [Space(10)]
    public Vector3 stageEventStartPosition;
    public Vector3 stageEventStartRotation;

    [Space(10)]
    public bool isStageEventActive = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void OnDisable() //OnDisable Unsubscribe from all events
    {
        StageEventManager.OnStageEventActive -= StageEventActive;
    }

    public virtual void StageEventPrepare()
    {
        if (stageEventTarget == null)
        {
            stageEventTarget = transform;
        }
    }

    public virtual void StageEventStart() //Called to Start the Stage Event and for it to start listening for Events
    {
        stageEventStartPosition = transform.position;
        stageEventStartRotation = transform.rotation.eulerAngles;

        StageEventManager.OnStageEventActive += StageEventActive;

        isStageEventActive = true;
    }

    public virtual void StageEventActive() //On - Off for Event
    {
        if (isStageEventActive)
        {
            isStageEventActive = false;
        }
        else
        {
            isStageEventActive = true;
        }
    }

    public virtual void StageEventAction() //Logic for during Event
    {
        if (isStageEventActive)
        {
            StageEventCompleted();
        }
    }

    public virtual void StageEventCompleted() //Called Once Event is completed
    {
        StageEventManager.OnStageEventActive -= StageEventActive;

        StageLightManager.Instance.ClearCurrentStaticStageLights();
        StageLightManager.Instance.ClearCurrentEventStageLights();

        StageEventManager.Instance.CompleteStageEvent();

        isStageEventActive = false;
    }

    public virtual void StageEventReset()
    {

    }

    public virtual void InteractWithStageEventUsingProp(GameObject propInteraction)
    {

    }
}
