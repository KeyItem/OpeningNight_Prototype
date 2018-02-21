using System.Collections;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    /*Alright, let's break it down.
     * The System needs to have a list of stage events in each scene, with their timings and relevant triggers.
     * It needs to know when to activate a stage event and when that stage event is completed.
    */

    public static StageEventManager Instance;

    [Header("Stage Events Attributes")]
    public StageEvent[] stageEvents;

    [Space(10)]
    public int stageEventIndex = 0;
    public StageEvent currentStageEvent = null;

    [Space(10)]
    public bool isFinishedAllStageEvents = false;

    public delegate void StageEventStart();
    public delegate void StageEventActive();
    public delegate void StageEventComplete();

    public static event StageEventStart OnStageEventStarted;
    public static event StageEventActive OnStageEventActive;
    public static event StageEventComplete OnStageEventCompleted;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private string targetDebugString;

    private void Awake()
    {
        InstanceSetup();
    }

    private void InstanceSetup()
    {
        Instance = this;
    }

    public void StartStageEvent()
    {
        if (currentStageEvent != null)
        {
            currentStageEvent.StageEventStart();

            if (canShowDebug)
            {
                targetDebugString = "SE :: " + currentStageEvent.stageEventName;
            }

            if (OnStageEventStarted != null)
            {
                Debug.Log("StageEvent :: Start :: Event");

                OnStageEventStarted();
            }
        }     
    }

    public void ActivateStageEvent()
    {
        if (canShowDebug)
        {

        }

        if (OnStageEventActive != null)
        {
            Debug.Log("StageEvent :: Active :: Event");

            OnStageEventActive();
        }
    }

    public void CompleteStageEvent()
    {
        if (canShowDebug)
        {
            targetDebugString = "";
        }

        if (OnStageEventCompleted != null)
        {
            OnStageEventCompleted();
        }

        currentStageEvent = null;
    }

    private void StartFirstStageEvent()
    {
        StageEvent firstStageEvent = stageEvents[0];
        stageEventIndex = 0;

        currentStageEvent = firstStageEvent;

        StartStageEvent();
    }

    public void RequestStageEvent()
    {
        MoveToNextStageEvent();
    }

    private void MoveToNextStageEvent()
    {
        if (CanMoveToNextStageEvent(stageEventIndex, stageEvents))
        {
            StageEvent newStageEvent = ReturnNextStageEvent(stageEvents);

            if (newStageEvent != null)
            {
                currentStageEvent = newStageEvent;

                StartStageEvent();
            }
        }
        else
        {
            isFinishedAllStageEvents = true;
        }
    }

    private bool CanMoveToNextStageEvent(int currentStageEventIndex, StageEvent[] stageEvents)
    {
        if (stageEvents.Length > 0)
        {
            currentStageEventIndex++;

            if (currentStageEventIndex <= stageEvents.Length - 1)
            {
                return true;
            }
        }

        return false;
    }

    private StageEvent ReturnNextStageEvent(StageEvent[] stageEvents)
    {
        return stageEvents[++stageEventIndex];
    }

    private StageEvent ReturnRequestedStageEvent(int stageEventIndex, StageEvent[] stageEvents)
    {
        if (stageEvents.Length - 1 >= stageEventIndex)
        {
            return stageEvents[stageEventIndex];
        }

        return null;
    }

    private void OnGUI()
    {
        if (canShowDebug)
        {
            GUI.Box(new Rect(35, 0, 300, 25), "");
            GUI.Label(new Rect(35, 0, 300, 25), targetDebugString);
        }       
    }
}
