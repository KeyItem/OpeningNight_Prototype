using System.Collections;
using UnityEngine;

public static class EaseHelper
{
    public static float EaseValue (float targetRatio, float targetEasingAmount)
    {
        float x = targetRatio;
        float a = targetEasingAmount + 1;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}
