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

    [Header("Stage Event Timing Attributes")]
    public float stageEventCompleteTime = 3f;
    private float stageEventCompleteCurrentTime;

    [HideInInspector]
    public bool isCountingEventTime = false;

    [Space(10)]
    public bool doesStageEventHaveTiming = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    public virtual void Update()
    {
        ManageStageEventTiming();
    }

    private void OnDisable() //OnDisable Unsubscribe from all events
    {
        StageEventManager.OnStageEventActive -= StageEventActive;
    }

    public virtual void ManageStageEventTiming()
    {
        if (doesStageEventHaveTiming)
        {
            if (isCountingEventTime)
            {
                stageEventCompleteCurrentTime -= Time.deltaTime;

                if (stageEventCompleteCurrentTime <= 0)
                {
                    StageEventFail();
                }
            }
            else
            {
                stageEventCompleteCurrentTime = stageEventCompleteTime;
            }
        }
    }

    public virtual void StageEventPrepare()
    {
        if (stageEventTarget == null)
        {
            stageEventTarget = transform;
        }

        stageEventCompleteCurrentTime = stageEventCompleteTime;
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

    public virtual void StageEventFail()
    {
        StageEventReset();
    }

    public virtual void StageEventReset()
    {
        stageEventCompleteCurrentTime = stageEventCompleteTime;

        isCountingEventTime = false;

        transform.position = stageEventStartPosition;
        transform.rotation = Quaternion.Euler(stageEventStartRotation);
    }

    public virtual void InteractWithStageEventUsingProp(GameObject propInteraction)
    {

    }
}
