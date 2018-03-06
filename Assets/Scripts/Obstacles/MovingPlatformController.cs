using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : RaycastController2D
{
    [Header("Platform Attributes")]
    public Vector2 move;

    [Space(10)]
    public LayerMask passengerMask;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void Start()
    {
        base.RaycastSetup();
    }

    private void Update()
    {
        UpdateObjectBounds();

        MovePlatform();
    }

    private void MovePlatform()
    {
        Vector2 velocity = move * Time.deltaTime;

        MovePassengers(velocity);

        transform.Translate(velocity);
    }

    private void MovePassengers(Vector2 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertical Platform
        if (velocity.y != 0)
        {
            float verticalRayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < playerCollisionVerticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? objectBounds.bottomLeft : objectBounds.topLeft;
                rayOrigin += Vector2.right * (playerCollisionVerticalRaySpacing * i);

                RaycastHit2D verticalHit2D = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, verticalRayLength, passengerMask);

                if (verticalHit2D)
                {
                    if (!movedPassengers.Contains(verticalHit2D.transform))
                    {
                        movedPassengers.Add(verticalHit2D.transform);

                        float pushX = (directionY == 1) ? pushX = velocity.x : 0;
                        float pushY = velocity.y - (verticalHit2D.distance - skinWidth) * directionY;

                        verticalHit2D.transform.Translate(new Vector2(pushX, pushY));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.up * directionY) * verticalRayLength, Color.red);
                }
            }

        }

        //Horizontal Platform
        if (velocity.x != 0)
        {
            float horizontalRayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < playerCollisionHorizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
                rayOrigin += Vector2.up * (playerCollisionHorizontalRaySpacing * i);

                RaycastHit2D horizontalHit2D = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, horizontalRayLength, passengerMask);

                if (horizontalHit2D)
                {
                    if (!movedPassengers.Contains(horizontalHit2D.transform))
                    {
                        movedPassengers.Add(horizontalHit2D.transform);

                        float pushX = velocity.x - (horizontalHit2D.distance - skinWidth) * directionX;
                        float pushY = 0;

                        horizontalHit2D.transform.Translate(new Vector2(pushX, pushY));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.right * directionX) * horizontalRayLength, Color.red);
                }
            }
        }

        //If Passengers on top of horizontal or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float verticalRayLength = skinWidth * 2;

            for (int i = 0; i < playerCollisionVerticalRayCount; i++)
            {
                Vector2 rayOrigin = objectBounds.topLeft + Vector2.right * (playerCollisionVerticalRaySpacing * i);
                RaycastHit2D verticalHit2D = Physics2D.Raycast(rayOrigin, Vector2.up, verticalRayLength, passengerMask);

                if (verticalHit2D)
                {
                    if (!movedPassengers.Contains(verticalHit2D.transform))
                    {
                        movedPassengers.Add(verticalHit2D.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        verticalHit2D.transform.Translate(new Vector2(pushX, pushY));
                    }
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.up * directionY) * verticalRayLength, Color.red);
                }
            }
        }
    }
}
