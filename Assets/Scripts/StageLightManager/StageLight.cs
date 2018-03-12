using System.Collections;
using UnityEngine;

public class StageLight : MonoBehaviour
{
    [Header("Stage Light Attributes")]
    public Light stageLight;

    [Space(10)]
    public STAGELIGHT_TYPE stageLightType;

    [Space(10)]
    public STAGELIGHT_ACTION stageLightAction;

    [Header("Stage Light Default Attributes")]
    public Color defaultStageLightColor;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private IEnumerator StageLightCoroutine = null;

    [Header("Stage Light Follow Attributes")]
    private Transform followTransform;

    private void Awake()
    {
        SetupStageLightDefaults();
    }

    private void Start()
    {
        if (stageLight == null)
        {
            stageLight = GetComponent<Light>();
        }
    }

    private void Update()
    {
        ManageStageLight();
    }

    private void SetupStageLightDefaults()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    private void ManageStageLight()
    {
        if (stageLightAction == STAGELIGHT_ACTION.LOOKAT)
        {
            StageLightFollowTarget();
        }
    }

    public void StageLightChangeColor(Color newColor)
    {
        stageLightAction = STAGELIGHT_ACTION.CHANGE_COLOR;

        if (newColor != Color.clear)
        {
            stageLight.color = newColor;
        }
    }

    public void StartStageLightPulse(LightPulseAttributes lightPulseAttributes)
    {
        stageLightAction = STAGELIGHT_ACTION.PULSE;

        StartCoroutine(StageLightPulser(lightPulseAttributes));
    }

    public void TurnOnStageLight(Color newColor)
    {
        stageLightAction = STAGELIGHT_ACTION.TURN_ON;

        if (newColor != Color.clear)
        {
            stageLight.color = newColor;
        }

        stageLight.enabled = true;
    }

    public void TurnOffStageLight()
    {
        stageLightAction = STAGELIGHT_ACTION.TURN_OFF;

        stageLight.enabled = false;
    }

    public void SetNewStageLightFollowTarget(Transform newTarget, Color newColor)
    {
        stageLightAction = STAGELIGHT_ACTION.LOOKAT;

        if (newColor != Color.clear)
        {
            stageLight.color = newColor;
        }

        followTransform = newTarget;
    }

    public void StageLightMoveToPosition(Vector3 newPosition, Quaternion newRotation, Color newColor)
    {
        stageLightAction = STAGELIGHT_ACTION.MOVETO;

        if (newColor != Color.clear)
        {
            stageLight.color = newColor;
        }

        transform.position = newPosition;
        transform.rotation = newRotation;
    }

    public void SetNewStageLightRotatePoints(Transform[] rotatePoints, float rotateSpeed, Color newColor)
    {
        stageLightAction = STAGELIGHT_ACTION.ROTATE;

        if (newColor != Color.clear)
        {
            stageLight.color = newColor;
        }

        StartCoroutine(RotateStageLights(rotatePoints, rotateSpeed));
    }

    public void ResetStageLight()
    {
        stageLight.color = defaultStageLightColor;

        stageLightAction = STAGELIGHT_ACTION.NONE;

        followTransform = null;

        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
    }

    private void StageLightFollowTarget()
    {
        transform.LookAt(followTransform);
    }

    private IEnumerator StageLightPulser(LightPulseAttributes lightPulseAttributes)
    {
        int pulseCount = lightPulseAttributes.newColors.Length;

        Light targetLight = stageLight;

        for (int i = 0; i < pulseCount; i++)
        {
            float startTime = Time.time;

            Color newColor = lightPulseAttributes.newColors[i];
            float waitTime = lightPulseAttributes.pulseTimes[i];
            float inbetweenTime = lightPulseAttributes.pulseInbetweenTimes[i];

            targetLight.color = newColor;

            if (waitTime > 0)
            {
                while (Time.time < startTime + waitTime)
                {
                    yield return null;
                }
            }

            if (inbetweenTime > 0)
            {
                startTime = Time.time;

                targetLight.color = Color.black;

                while (Time.time < startTime + inbetweenTime)
                {
                    yield return null;
                }
            }
        }

        ResetStageLight();

        yield return null;
    }

    private IEnumerator RotateStageLights(Transform[] targetRotationPoints, float rotateSpeed)
    {
        transform.rotation = targetRotationPoints[0].rotation;

        for (int i = 1; i < targetRotationPoints.Length; i++)
        {
            Quaternion startRotation = transform.rotation;

            float newRotateTime = 0f;

            while (newRotateTime < 1f)
            {
                newRotateTime += (Time.deltaTime / rotateSpeed);

                if (newRotateTime > 1f)
                {
                    newRotateTime = 1f;
                }

                transform.rotation = Quaternion.Lerp(startRotation, targetRotationPoints[i].rotation, newRotateTime);

                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }
}

[System.Serializable]
public struct StageLightTargets
{
    public Transform[] targets;
}
