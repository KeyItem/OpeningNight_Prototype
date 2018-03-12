using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Light Command Data", menuName = "Light/Light Command Data", order = 1401)]
public class StageLightCommandData : ScriptableObject
{
    [Header("Lighting Command Attributes")]
    public STAGELIGHT_TYPE[] lightType = new STAGELIGHT_TYPE[4];

    [Space(10)]
    public STAGELIGHT_ACTION[] lightCommands = new STAGELIGHT_ACTION[4];

    [Space(10)]
    public int[] targetLights = new int[4];

    [Space(10)]
    public Color[] targetLightColors = new Color[4];

    [Space(10)]
    public float[] targetStageLightTimes = new float[4];

    [Header("Light Pulse Command Attributes")]
    public LightPulseAttributes[] lightPulseAttributes;
}

public enum STAGELIGHT_ACTION
{
    NONE,
    MOVETO,
    ROTATE,
    CHANGE_COLOR,
    PULSE,
    LOOKAT,
    TURN_ON,
    TURN_OFF
}

public enum STAGELIGHT_TYPE
{
    NONE,
    STATIC,
    DYNAMIC
}
