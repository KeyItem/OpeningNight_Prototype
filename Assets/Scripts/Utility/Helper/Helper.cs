using System.Collections;
using UnityEngine;

public static class Helper
{
    public static float SortByLowToHigh(float firstValue, float secondValue)
    {
        return firstValue.CompareTo(secondValue);
    }
}
