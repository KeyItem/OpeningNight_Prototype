using System.Collections;
using UnityEngine;

[System.Serializable]
public class StageEvent : MonoBehaviour
{
    [Header("Base Stage Event Attributes")]
    public string stageEventName;

    [Space(10)]
    public Transform stageEventTransform;

    [Space(10)]
    public GameObject[] stageObjects;

    [Space(10)]
    public Color stageLightColor = Color.yellow;

    [Space(10)]
    public bool isStageEventActive = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void OnDisable() //OnDisable Unsubscribe from all events
    {
        StageEventManager.OnStageEventActive -= StageEventActive;
    }

    public virtual void StageEventStart() //Called to Start the Stage Event and for it to start listening for Events
    {
        StageEventManager.OnStageEventActive += StageEventActive;

        if (stageEventTransform == null)
        {
            stageEventTransform = transform;
        }

        StageLightManager.Instance.RequestStageLight(stageEventTransform.position, Quaternion.identity, stageLightColor);

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

        StageEventManager.Instance.CompleteStageEvent();

        StageLightManager.Instance.ClearCurrentStageLights();

        isStageEventActive = false;
    }

    public virtual void InteractWithStageEventUsingProp(GameObject propInteraction)
    {

    }

    public virtual void EnableStageObjects()
    {
        if (stageObjects.Length > 0)
        {
            for (int i = 0; i < stageObjects.Length; i++)
            {
                stageObjects[i].SetActive(true);
            }
        }
    }

    public virtual void DisableStageObjects()
    {
        if (stageObjects.Length > 0)
        {
            for (int i = 0; i < stageObjects.Length; i++)
            {
                stageObjects[i].SetActive(false);
            }
        }
    }
}
