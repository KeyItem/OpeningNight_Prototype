using System.Collections;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    /*Alright, let's break it down.
     * The System needs to have a list of stage events in each scene, with their timings and relevant triggers.
     * It needs to know when to activate a stage event and when that stage event is completed.
    */

    public static StageEventManager instance;

    [Header("Stage Events Attributes")]
    public StageEvent[] stageEvents;

    [Space(10)]
    public int stageEventIndex = 0;
    public StageEvent currentStageEvent = null;

    public delegate void StageEventStart();
    public delegate void StageEventActive();
    public delegate void StageEventComplete();

    public static event StageEventStart OnStageEventStarted;
    public static event StageEventActive OnStageEventActive;
    public static event StageEventComplete OnStageEventCompleted;

    private void Awake()
    {
        InstanceSetup();
    }

    private void Start()
    {
        StartFirstStageEvent();
    }

    private void Update()
    {
        DebugInput();
    }

    private void InstanceSetup()
    {
        instance = this;
    }

    private void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToNextStageEvent();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateStageEvent();
        }
    }

    public void StartStageEvent()
    {
        if (currentStageEvent != null)
        {
            currentStageEvent.StageEventStart();

            if (OnStageEventStarted != null)
            {
                Debug.Log("StageEvent :: Start");

                OnStageEventStarted();
            }
        }     
    }

    public void ActivateStageEvent()
    {     
        if (OnStageEventActive != null)
        {
            Debug.Log("StageEvent :: Active");

            OnStageEventActive();
        }
    }

    public void CompleteStageEvent()
    {
        if (OnStageEventCompleted != null)
        {
            Debug.Log("StageEvent :: Completed");

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
}
