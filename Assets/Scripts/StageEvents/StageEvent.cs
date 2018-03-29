using System.Collections;
using UnityEngine;

[System.Serializable]
public class StageEvent : MonoBehaviour
{
    [Header("Base Stage Event Attributes")]
    public string stageEventName;

    [Space(10)]
    public STAGE_EVENT_TYPE stageEventType;

    [Space(10)]
    public Transform stageEventTarget;

    [HideInInspector]
    public Vector3 stageEventStartPosition;
    [HideInInspector]
    public Vector3 stageEventStartRotation;

    [Space(10)]
    public bool isStageEventActive = false;

    [Space(10)]
    public bool isStageEventCompleted = false;

    [Header("Stage Event Fail Timing Attributes")]
    public float stageEventFailTime = 3f;
    private float stageEventCurrentFailTime;

    [HideInInspector]
    public bool isStageEventCountingFailTime = false;

    [Space(10)]
    public bool stageEventHasFailTime = false;

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
        if (isStageEventActive)
        {
            if (stageEventHasFailTime)
            {
                if (isStageEventCountingFailTime)
                {
                    stageEventCurrentFailTime -= Time.deltaTime;

                    if (stageEventCurrentFailTime <= 0)
                    {
                        StageEventFail();
                    }
                }
                else
                {
                    stageEventCurrentFailTime = stageEventFailTime;
                }
            }
        }       
    }

    public virtual void StageEventPrepare()
    {
        if (stageEventTarget == null)
        {
            stageEventTarget = transform;
        }

        stageEventCurrentFailTime = stageEventFailTime;
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

        isStageEventCompleted = true;
    }

    public virtual void StageEventFail()
    {
        if (stageEventType == STAGE_EVENT_TYPE.REQUIRED)
        {
            StageEventReset();
        }
        else if (stageEventType == STAGE_EVENT_TYPE.OPTIONAL)
        {
            StageEventCompleted();
        }
    }

    public virtual void StageEventReset()
    {
        stageEventCurrentFailTime = stageEventFailTime;

        isStageEventCountingFailTime = false;

        transform.position = stageEventStartPosition;
        transform.rotation = Quaternion.Euler(stageEventStartRotation);
    }

    public virtual void InteractWithStageEventUsingProp(GameObject propInteraction)
    {

    }
}

public enum STAGE_EVENT_TYPE
{
    NONE,
    REQUIRED,
    OPTIONAL
}
