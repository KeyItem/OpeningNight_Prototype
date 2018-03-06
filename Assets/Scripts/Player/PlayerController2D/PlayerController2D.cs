using System.Collections;
using UnityEngine;

public class PlayerController2D : RaycastController2D
{
    [Header("Player Input Values")]
    public Vector2 playerInput;

    [Header("Player Movement Attributes")]
    public float playerBaseMoveSpeed;

    private float playerTargetMoveSpeed;

    [Space(10)]
    public float playerMoveSmoothingGround = 0.12f;
    public float playerMoveSmoothingAir = 0.25f;

    private float playerMoveSmoothVelocity;

    private Vector3 playerVelocity;

    [Header("Player Jumping Attributes")]
    public float playerJumpHeight;
    public float playerJumpTime;

    public Vector2 playerJumpVelocity;

    [Header("Player Gravity Attributes")]
    public float playerGravity;

    public Vector2 playerGravityVelocity;

    [Header("Player Collision Attributes")]
    public PlayerCollisionInfo playerCollisionData;

    [Space(10)]
    public LayerMask groundMask;

    [Header("Player Slope Attributes")]
    public float maxClimbAngle = 75f;
    public float maxDescentAngle = 75f;

    [Space(10)]
    public bool isOnSlope = false;

    [Header("DEBUG")]
    public bool canShowDebug = true;

    private void Start()
    {
        base.RaycastSetup();
        
        PlayerGravitySetup();
    }

    private void PlayerGravitySetup()
    {
        playerGravity = -(2 * playerJumpHeight / Mathf.Pow(playerJumpTime, 2));
        playerJumpVelocity.y = Mathf.Abs(playerGravity) * playerJumpTime;
    }

    public void ReceiveInput(Vector2 receivedPlayerInput, bool hasJumped)
    {
        playerInput = receivedPlayerInput;

        CalculateVelocity(receivedPlayerInput, hasJumped);
    }

    private void CalculateVelocity(Vector2 receivedPlayerInput, bool hasJumped)
    {
        playerTargetMoveSpeed = receivedPlayerInput.x * playerBaseMoveSpeed;

        playerVelocity.x = Mathf.SmoothDamp(playerVelocity.x, playerTargetMoveSpeed, ref playerMoveSmoothVelocity, ReturnMovementSmoothing());

        if (playerCollisionData.isCollisionAbove || playerCollisionData.isCollisionBelow)
        {
            playerVelocity.y = 0;
        }

        if (hasJumped)
        {
            if (playerCollisionData.isCollisionBelow)
            {
                playerVelocity.y = playerJumpVelocity.y;
            }
        }

        playerVelocity.y += playerGravity * Time.deltaTime;

        PlayerMove(playerVelocity * Time.deltaTime);
    }

    public void PlayerMove(Vector2 finalPlayerVelocity)
    {
        UpdateObjectBounds();

        if (finalPlayerVelocity.y < 0)
        {
            DescentSlope(ref finalPlayerVelocity);
        }

        ManageCollisions(ref finalPlayerVelocity);

        transform.Translate(finalPlayerVelocity);
    }

    public void PlayerJump()
    {
        if (playerCollisionData.isCollisionBelow)
        {
            playerVelocity.y = playerJumpVelocity.y;

            Debug.Log("Jump");
        }
    }

    private void ManageCollisions(ref Vector2 playerVelocity)
    {
        playerCollisionData.ResetCollisions();

        playerCollisionData.velocityOld = playerVelocity;

        if (playerVelocity.x != 0)
        {
            DetectHorizontalCollisions(ref playerVelocity);
        }

        if (playerVelocity.y != 0)
        {
            DetectVerticalCollisions(ref playerVelocity);
        }
    }

    private void ClimbSlope(ref Vector2 playerVelocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(playerVelocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (playerVelocity.y <= climbVelocityY)
        {
            playerVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(playerVelocity.x);
            playerVelocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            playerCollisionData.isCollisionBelow = true;
            playerCollisionData.isClimbingSlope = true;
            playerCollisionData.currentSlopeAngle = slopeAngle;
        }
    }

    private void DescentSlope(ref Vector2 playerVelocity)
    {
        float directionX = Mathf.Sign(playerVelocity.x);

        Vector2 rayOrigin = (directionX == -1) ? objectBounds.bottomRight : objectBounds.bottomLeft;

        RaycastHit2D descentHit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, groundMask);

        if (descentHit)
        {
            float slopeAngle = Vector2.Angle(descentHit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= maxDescentAngle)
            {
                if (Mathf.Sign(descentHit.normal.x) == directionX)
                {
                    if (descentHit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad * Mathf.Abs(playerVelocity.x)))
                    {
                        float descendDistance = Mathf.Abs(playerVelocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * descendDistance;

                        playerVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * descendDistance * Mathf.Sign(playerVelocity.x);
                        playerVelocity.y -= descendVelocityY;

                        playerCollisionData.isCollisionBelow = true;
                        playerCollisionData.currentSlopeAngle = slopeAngle;
                        playerCollisionData.isDescendingSlope = true;
                    }
                }
            }
        }
    }

    private void DetectHorizontalCollisions(ref Vector2 playerVelocity)
    {
        float directionX = Mathf.Sign(playerVelocity.x);
        float rayLength = Mathf.Abs(playerVelocity.x) + skinWidth;

        for (int i = 0; i < playerCollisionHorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rayOrigin = objectBounds.bottomLeft : objectBounds.bottomRight;
            rayOrigin += Vector2.up * (playerCollisionHorizontalRaySpacing * i);

            RaycastHit2D horizontalHit2D = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, groundMask);

            if (horizontalHit2D)
            {
                float slopeAngle = Vector2.Angle(horizontalHit2D.normal, Vector2.up);

                if (i == 0)
                {
                    if (slopeAngle <= maxClimbAngle)
                    {
                        if (playerCollisionData.isDescendingSlope)
                        {
                            playerCollisionData.isDescendingSlope = false;
                            playerVelocity = playerCollisionData.velocityOld;
                        }

                        float distanceToSlopeStart = 0;

                        if (slopeAngle != playerCollisionData.slopeAngleOld)
                        {
                            distanceToSlopeStart = horizontalHit2D.distance - skinWidth;

                            playerVelocity.x -= distanceToSlopeStart * directionX;
                        }

                        ClimbSlope(ref playerVelocity, slopeAngle);

                        playerVelocity.x += distanceToSlopeStart * directionX;
                    }
                }

                if (!playerCollisionData.isClimbingSlope || slopeAngle > maxClimbAngle)
                {
                    playerVelocity.x = (horizontalHit2D.distance - skinWidth) * directionX;
                    rayLength = horizontalHit2D.distance;

                    if (playerCollisionData.isClimbingSlope)
                    {
                        playerVelocity.y = Mathf.Tan(playerCollisionData.currentSlopeAngle * Mathf.Deg2Rad * Mathf.Abs(playerVelocity.x));
                    }

                    playerCollisionData.isCollisionRight = directionX == 1;
                    playerCollisionData.isCollisionLeft = directionX == -1;
                }

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.right * directionX) * rayLength, Color.red);
                }
            }
            else
            {
                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.right * directionX) * 2, Color.green);
                }
            }
        }
    }

    private void DetectVerticalCollisions(ref Vector2 playerVelocity)
    {
        float directionY = Mathf.Sign(playerVelocity.y);
        float rayLength = Mathf.Abs(playerVelocity.y) + skinWidth;

        for (int i = 0; i < playerCollisionVerticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? objectBounds.bottomLeft : objectBounds.topLeft;
            rayOrigin += Vector2.right * (playerCollisionVerticalRaySpacing * i + playerVelocity.x);

            RaycastHit2D verticalHit2D = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, groundMask);

            if (verticalHit2D)
            {
                playerVelocity.y = (verticalHit2D.distance - skinWidth) * directionY;
                rayLength = verticalHit2D.distance;

                if (playerCollisionData.isClimbingSlope)
                {
                    playerVelocity.x = playerVelocity.y / Mathf.Tan(playerCollisionData.currentSlopeAngle * Mathf.Rad2Deg) * Mathf.Sign(playerVelocity.x);
                }

                playerCollisionData.isCollisionAbove = directionY == 1;
                playerCollisionData.isCollisionBelow = directionY == -1;

                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, (Vector2.up * directionY) * rayLength, Color.red);
                }
            }
            else
            {
                if (canShowDebug)
                {
                    Debug.DrawRay(rayOrigin, Vector2.up * -2, Color.green);
                }
            } 
            
            if (playerCollisionData.isClimbingSlope)
            {
                float directionX = Mathf.Sign(playerVelocity.x);

                rayLength = Mathf.Abs(playerVelocity.x) + skinWidth;

                Vector2 newRayOrigin = ((directionX == -1) ? objectBounds.bottomLeft : objectBounds.bottomRight) + Vector2.up * playerVelocity.y;

                RaycastHit2D newHit = Physics2D.Raycast(newRayOrigin, Vector2.right * directionX, rayLength, groundMask);

                if (newHit)
                {
                    float newSlopeAngle = Vector2.Angle(newHit.normal, Vector2.up);

                    if (newSlopeAngle != playerCollisionData.currentSlopeAngle)
                    {
                        playerVelocity.x = (newHit.distance - skinWidth) * directionX;

                        playerCollisionData.currentSlopeAngle = newSlopeAngle;
                    }
                }
            }
        }
    }

    private float ReturnMovementSmoothing()
    {
        if (!playerCollisionData.isCollisionBelow)
        {
            return playerMoveSmoothingAir;
        }
        else
        {
            return playerMoveSmoothingGround;
        }
    }
}

[System.Serializable]
public struct PlayerCollisionInfo
{
    public bool isCollisionAbove;
    public bool isCollisionBelow;
    public bool isCollisionLeft;
    public bool isCollisionRight;

    public bool isClimbingSlope;
    public bool isDescendingSlope;

    public float currentSlopeAngle;
    public float slopeAngleOld;

    public Vector2 velocityOld;

    public void ResetCollisions()
    {
        isCollisionAbove = isCollisionBelow = false;
        isCollisionLeft = isCollisionRight = false;
        isClimbingSlope = false;
        isDescendingSlope = false;
        slopeAngleOld = currentSlopeAngle;
        currentSlopeAngle = 0f;
    }
}
