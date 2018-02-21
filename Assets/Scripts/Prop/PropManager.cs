using System.Collections;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public static PropManager propManager;

    [Header("Prop Manager Attributes")]
    public PropController[] propsToSetup;

    [Space(10)]
    public PropPoints[] propPoints;

    [Space(10)]
    public Transform propSpawnPoint;

    [Space(10)]
    public bool arePropsSetup = false;

    private void Awake()
    {
        InitializePropManager();
    }

    private void Start()
    {
        PrepareProps();
    }

    private void Update()
    {
        DebugInput();
    }

    private void InitializePropManager()
    {
        propManager = this;
    }

    private void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetupAllProps();
        }
    }

    private void PrepareProps()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            propsToSetup[i].transform.position = propSpawnPoint.position;

            propsToSetup[i].gameObject.SetActive(false);
        }
    }

    public void SetupAllProps()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            if (!propsToSetup[i].isPropInitialized)
            {
                SetupProp(propsToSetup[i], propPoints[i]);
            }
        }
    }

    public void SetupTargetProp(int propIndex)
    {
        SetupProp(propsToSetup[propIndex], propPoints[propIndex]);
    }

    private void SetupProp(PropController propController, PropPoints newPropPoints)
    {
        propController.gameObject.SetActive(true);

        propController.StartSetup(newPropPoints);
    }
}
