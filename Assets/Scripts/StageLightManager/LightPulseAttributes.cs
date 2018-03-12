using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Light Pulse Attributes", menuName = "Light/Light Pulse Attributes", order = 1402)]
public class LightPulseAttributes : ScriptableObject
{
    [Header("Light Pulse Attributes")]
    public Color[] newColors;

    [Space(10)]
    public float[] pulseTimes;

    [Space(10)]
    public float[] pulseInbetweenTimes;
}

