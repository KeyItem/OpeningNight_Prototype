using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLightManager : MonoBehaviour
{
    private static StageLightManager _instance;
    public static StageLightManager Instance { get { return _instance; } }

    private const string lightIdentifier = "StageLight";

    [Header("Stage Light Manager Attributes")]
    public StageLight[] staticStageLights;

    [Space(10)]
    public List<StageLight> activeStaticStageLights = new List<StageLight>(4);
    public List<GameObject> activeEventStageLights;

    [Space(10)]
    public float lightRestingHeight = 10f;

    [Header("Light Command Data Attributes")]
    public StageLightCommandData currentLightCommandData;

    private void Awake()
    {
        InitializeStageLightManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ParseStageLightingCommandData(currentLightCommandData, null);
        }
    }

    private void InitializeStageLightManager()
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

    public void ParseStageLightingCommandData(StageLightCommandData newLightingCommandData, StageLightTargets[] stageLightTargets)
    {
        if (newLightingCommandData != null)
        {
            currentLightCommandData = newLightingCommandData;

            for (int i = 0; i < newLightingCommandData.lightCommands.Length; i++)
            {
                if (newLightingCommandData.lightCommands[i] == STAGELIGHT_ACTION.NONE)
                {
                    continue;
                }

                StageLight targetStageLight = null;

                if (newLightingCommandData.lightType[i] == STAGELIGHT_TYPE.STATIC)
                {
                    targetStageLight = staticStageLights[newLightingCommandData.targetLights[i]];

                    activeStaticStageLights.Add(targetStageLight);
                }
                else if (newLightingCommandData.lightType[i] == STAGELIGHT_TYPE.DYNAMIC)
                {
                    targetStageLight = RequestEventStageLight();
                }

                StageLightTargets targetTransformData = stageLightTargets[i];

                Light targetLight = targetStageLight.stageLight;

                Color newColor = newLightingCommandData.targetLightColors[i];

                targetStageLight.ResetStageLight();

                switch (newLightingCommandData.lightCommands[i])
                {
                    case STAGELIGHT_ACTION.PULSE:
                        targetStageLight.StartStageLightPulse(newLightingCommandData.lightPulseAttributes[i]);
                        break;

                    case STAGELIGHT_ACTION.LOOKAT:
                        if (targetTransformData.targets != null)
                        {
                            targetStageLight.SetNewStageLightFollowTarget(targetTransformData.targets[0], newLightingCommandData.targetLightColors[i]);
                        }
                        break;

                    case STAGELIGHT_ACTION.FOLLOW_AND_LOOKAT:
                        if (targetTransformData.targets != null)
                        {
                            targetStageLight.SetNewFollowAndLookAtTarget(targetTransformData.targets[0], newLightingCommandData.targetLightColors[i]);
                        }
                        break;

                    case STAGELIGHT_ACTION.MOVETO:
                        if (targetTransformData.targets != null)
                        {
                            targetStageLight.StageLightMoveToPosition(targetTransformData.targets[0].position, targetTransformData.targets[0].rotation, newLightingCommandData.targetLightColors[i]);
                        }
                        break;

                    case STAGELIGHT_ACTION.ROTATE:
                        if (targetTransformData.targets != null)
                        {
                            targetStageLight.SetNewStageLightRotatePoints(targetTransformData.targets, newLightingCommandData.targetStageLightTimes[i], newLightingCommandData.targetLightColors[i]);
                        }              
                        break;

                    case STAGELIGHT_ACTION.CHANGE_COLOR:
                        targetStageLight.StageLightChangeColor(newLightingCommandData.targetLightColors[i]);
                        break;

                    case STAGELIGHT_ACTION.TURN_OFF:
                        targetStageLight.TurnOffStageLight();
                        break;

                    case STAGELIGHT_ACTION.TURN_ON:
                        targetStageLight.TurnOnStageLight(newLightingCommandData.targetLightColors[i]);
                        break;
                }
            }
        }
    }

    private void EnableStaticStageLight(Light newLight, Color newColor)
    {
        newLight.color = newColor;
        newLight.enabled = true;
    }

    private void DisableStaticStageLight(Light newLight)
    {
        newLight.enabled = false;
    }

    private void DisableAllStaticStageLights()
    {
        for (int i = 0; i < staticStageLights.Length; i++)
        {
            staticStageLights[i].TurnOffStageLight();
        }
    }

    private void EnableAllStaticStageLights()
    {
        for (int i = 0; i < staticStageLights.Length; i++)
        {
            staticStageLights[i].TurnOnStageLight(Color.clear);
        }
    }

    private StageLight RequestEventStageLight()
    {
        GameObject newLightObject = ObjectPooler.Instance.CreateObjectFromPool_Reuseable(lightIdentifier, Vector3.zero, Quaternion.identity);
        activeEventStageLights.Add(newLightObject);

        StageLight newStageLight = newLightObject.GetComponent<StageLight>();

        return newStageLight;
    }

    public void ClearCurrentStaticStageLights()
    {
        for (int i = 0; i < activeStaticStageLights.Count; i++)
        {
            StageLight currentActiveStageLight = activeStaticStageLights[i];

            currentActiveStageLight.ResetStageLight();
        }

        activeStaticStageLights.Clear();
    }

    public void ClearCurrentEventStageLights()
    {
        for (int i = 0; i < activeEventStageLights.Count; i++)
        {
            StageLight currentActiveStageLight = activeEventStageLights[i].GetComponent<StageLight>();

            currentActiveStageLight.ResetStageLight();

            ObjectPooler.Instance.ReturnObjectToQueue(lightIdentifier, activeEventStageLights[i]);
        }

        activeEventStageLights.Clear();
    }
}
