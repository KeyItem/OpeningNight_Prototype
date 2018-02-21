using System.Collections;
using UnityEngine;

public class PropController : MonoBehaviour
{
    [Header("Prop Controller Attributes")]
    public float propSetupTime = 2f;

    [Space(10)]
    public bool isPropInitialized = false;

    private PropPoints propPoints;

    private IEnumerator PropMoveToPosition;

    public void StartSetup(PropPoints newPropPoints)
    {
        propPoints = newPropPoints;

        if (PropMoveToPosition != null)
        {
            StopCoroutine(PropMoveToPosition);
        }

        PropMoveToPosition = MovePropToStartingPosition(propSetupTime);

        StartCoroutine(PropMoveToPosition);
    }

    private IEnumerator MovePropToStartingPosition(float propSetupTime)
    {
        if (propSetupTime > 0)
        {
            transform.position = propPoints.propStartingPosition.position;

            Vector3 startingPosition = propPoints.propStartingPosition.position;
            Vector3 endingPosition = propPoints.propEndingPosition.position;

            float setupTime = 0f;

            while (setupTime < 1f)
            {
                setupTime += Time.deltaTime / propSetupTime;

                if (setupTime > 1)
                {
                    setupTime = 1f;
                }

                Vector3 newPropPosition = Vector3.Slerp(startingPosition, endingPosition, setupTime);

                transform.position = newPropPosition;

                yield return new WaitForEndOfFrame();
            }
        }

        PropFinshedMoving();

        yield return null;
    }

    private void PropFinshedMoving()
    {
        isPropInitialized = true;
    }
}
