using System.Collections;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [Header("Prop Attributes")]
    public string propName;

    [Space(10)]
    public Transform targetProp;

    [Space(10)]
    public PropController propController;

    [Space(10)]
    public PropPoints propPoints;

    private void Start()
    {
        PropSetup();
    }

    private void PropSetup()
    {
        if (propController == null)
        {
            propController = GetComponent<PropController>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (propPoints.propStartingPosition != null && propPoints.propEndingPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(propPoints.propStartingPosition.position, propPoints.propEndingPosition.position);
        }
    }
}
