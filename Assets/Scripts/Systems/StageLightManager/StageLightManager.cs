using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLightManager : MonoBehaviour
{
    private static StageLightManager _instance;
    public static StageLightManager Instance { get { return _instance; } }

    private const string lightIdentifier = "StageLight";

    [Header("Stage Light Manager Attributes")]
    public List<GameObject> activeStageLights;

    [Space(10)]
    public float lightRestingHeight = 10f;

    private void Awake()
    {
        InitializeStageLightManager();
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

    public void RequestStageLight(Vector3 lightPosition, Quaternion lightRotation, Color lightColor)
    {
        Vector3 modifiedLightPosition = lightPosition;
        modifiedLightPosition.y = lightRestingHeight;

        if (lightRotation == Quaternion.identity)
        {
            lightRotation = Quaternion.Euler(90, 0, 0);
        }

        GameObject newLightObject = ObjectPooler.Instance.CreateObjectFromPool_Reuseable(lightIdentifier, modifiedLightPosition, lightRotation);
        activeStageLights.Add(newLightObject);

        Light newLight = newLightObject.GetComponent<Light>();
        newLight.color = lightColor;
        newLight.enabled = true;
    }

    public void ClearCurrentStageLights()
    {
        for (int i = 0; i < activeStageLights.Count; i++)
        {
            ObjectPooler.Instance.ReturnObjectToQueue(lightIdentifier, activeStageLights[i]);
        }

        activeStageLights.Clear();
    }
}
