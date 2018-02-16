using System.Collections;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    [Header("Pan Camera Attributes")]
    public AXIS cameraAxis;

    [Space(10)]
    public Transform cameraTransform;

    [Space(10)]
    public Transform trackedObject;

    [Space(10)]
    public bool isCameraEnabled = true;

    private Vector3 panCameraAxis;
    private Vector3 cameraPreviousPosition;
    private Vector3 cameraNextPosition;

    [Header("Camera Movement Attributes")]
    public float cameraSmoothingValue;

    [Space(10)]
    public float cameraMovePercent = 0f;

    [Space(10)]
    [Range(0f, 1f)]
    public float cameraMinStartPercent = 0.20f;

    [Range(0f, 1f)]
    public float cameraMaxEndPercent = 1f;

    [Space(10)]
    public Transform cameraMinRange;
    public Transform cameraMaxRange;

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (isCameraEnabled)
        {
            Vector3 cameraCurrentPosition = cameraTransform.position;

            cameraMovePercent = ReturnPlayerCameraMovePercent();

            if (cameraMovePercent > cameraMinStartPercent && cameraMovePercent < cameraMaxEndPercent)
            {
                float cameraNewPositionAxisPosition = Mathf.Lerp(cameraMinRange.position.x, cameraMaxRange.position.x, cameraMovePercent);

                Vector3 newCameraPosition = new Vector3(cameraNewPositionAxisPosition, cameraCurrentPosition.y, cameraCurrentPosition.z);

                cameraTransform.position = newCameraPosition;
            }
            else
            {
                AnchorCamera();
            }
        }    
    }

    private void AnchorCamera()
    {
        Vector3 cameraCurrentPosition = cameraTransform.position;
        Vector3 newCameraPosition = Vector3.zero;

        float cameraAnchorPointAxis = 0f;
        float interpolationSmoothing = cameraSmoothingValue * Time.deltaTime;

        if (cameraMovePercent < cameraMinStartPercent)
        {
            switch(cameraAxis)
            {
                case AXIS.X:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.x, cameraMaxRange.position.x, cameraMinStartPercent);

                    newCameraPosition = new Vector3(cameraAnchorPointAxis, cameraCurrentPosition.y, cameraCurrentPosition.z);

                    cameraTransform.position = Vector3.Lerp(cameraCurrentPosition, newCameraPosition, interpolationSmoothing);
                    break;

                case AXIS.Y:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.y, cameraMaxRange.position.y, cameraMinStartPercent);

                    newCameraPosition = new Vector3(cameraCurrentPosition.x, cameraAnchorPointAxis, cameraCurrentPosition.z);

                    cameraTransform.position = Vector3.Lerp(cameraCurrentPosition, newCameraPosition, interpolationSmoothing);
                    break;

                case AXIS.Z:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.z, cameraMaxRange.position.z, cameraMinStartPercent);

                    newCameraPosition = new Vector3(cameraCurrentPosition.x, cameraCurrentPosition.y, cameraAnchorPointAxis);

                    cameraTransform.position = Vector3.Lerp(cameraCurrentPosition, newCameraPosition, interpolationSmoothing);
                    break;
            }
        }
        else if (cameraMovePercent > cameraMaxEndPercent)
        {
            switch (cameraAxis)
            {
                case AXIS.X:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.x, cameraMaxRange.position.x, cameraMaxEndPercent);

                    newCameraPosition = new Vector3(cameraAnchorPointAxis, cameraCurrentPosition.y, cameraCurrentPosition.z);

                    cameraTransform.position = newCameraPosition;
                    break;

                case AXIS.Y:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.y, cameraMaxRange.position.y, cameraMaxEndPercent);

                    newCameraPosition = new Vector3(cameraCurrentPosition.x, cameraAnchorPointAxis, cameraCurrentPosition.z);

                    cameraTransform.position = newCameraPosition;
                    break;

                case AXIS.Z:
                    cameraAnchorPointAxis = Mathf.Lerp(cameraMinRange.position.z, cameraMaxRange.position.z, cameraMaxEndPercent);

                    newCameraPosition = new Vector3(cameraCurrentPosition.x, cameraCurrentPosition.y, cameraAnchorPointAxis);

                    cameraTransform.position = newCameraPosition;
                    break;
            }
        }
    }

    private float ReturnPlayerCameraMovePercent()
    {
        float newCameraMovePercent = 0f;

        switch (cameraAxis)
        {
            case AXIS.X:
                newCameraMovePercent = (trackedObject.position.x - cameraMinRange.position.x) / (cameraMaxRange.position.x - cameraMinRange.position.x);
                break;

            case AXIS.Y:
                newCameraMovePercent = (trackedObject.position.y - cameraMinRange.position.y) / (cameraMaxRange.position.y - cameraMinRange.position.y);
                break;

            case AXIS.Z:
                newCameraMovePercent = (trackedObject.position.z - cameraMinRange.position.z) / (cameraMaxRange.position.z - cameraMinRange.position.z);
                break;
        }

        newCameraMovePercent = Mathf.Clamp(newCameraMovePercent, 0, 1f);

        return newCameraMovePercent;
    }
}
