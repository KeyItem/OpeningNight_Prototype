using System.Collections;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    private static PropManager _instance;
    public static PropManager Instance { get { return _instance; } }

    [Header("Prop Manager Attributes")]
    public Prop[] propsToSetup;

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

    private void InitializePropManager()
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

    private void PrepareProps()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            propsToSetup[i].propController.gameObject.SetActive(false);
        }
    }

    public void StartPropSetup()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            if (!propsToSetup[i].propController.isPropInitialized)
            {
                SetupProp(propsToSetup[i].propController, propsToSetup[i].propPoints);
            }
        }
    }

    public void SetupTargetProp(int propIndex)
    {
        SetupProp(propsToSetup[propIndex].propController, propsToSetup[propIndex].propPoints);
    }

    private void SetupProp(PropController propController, PropPoints newPropPoints)
    {
        propController.gameObject.SetActive(true);

        propController.StartSetup(newPropPoints);
    }
}

[System.Serializable]
public struct PropPoints
{
    [Header("Prop Points")]
    public Transform propStartingPosition;
    public Transform propEndingPosition;
}
