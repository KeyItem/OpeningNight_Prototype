using System.Collections;
using UnityEngine;

public class StageEvent : MonoBehaviour
{
    [Header("Base Stage Event Attributes")]
    public string stageEventName;

    [Space(10)]
    public GameObject[] stageEventObjects;

    [Space(10)]
    public bool isStageEventActive = true;

    private void OnDisable() //OnDisable Unsubscribe from all events
    {
        StageEventManager.OnStageEventActive -= StageEventActive;
    }

    public virtual void StageEventStart() //Called to Start the Stage Event and for it to start listening for Events
    {
        Debug.Log("Stage Event Started :: " + gameObject.name);

        StageEventManager.OnStageEventActive += StageEventActive;

        isStageEventActive = true;
    }

    public virtual void StageEventActive() //On - Off for Event
    {
        if (isStageEventActive)
        {
            Debug.Log("Stage Event Deactivated :: " + gameObject.name);

            isStageEventActive = false;
        }
        else
        {
            Debug.Log("Stage Event Activated :: " + gameObject.name);

            isStageEventActive = true;
        }
    }

    public virtual void StageEventAction() //Logic for during Event
    {

    }

    public virtual void StageEventCompleted() //Called Once Event is completed
    {
        Debug.Log("Stage Event Completed :: " + gameObject.name);

        StageEventManager.OnStageEventActive -= StageEventActive;

        StageEventManager.instance.CompleteStageEvent();

        isStageEventActive = false;
    }
}
