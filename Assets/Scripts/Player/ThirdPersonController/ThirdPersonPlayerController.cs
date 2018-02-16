using System.Collections;
using UnityEngine;

[RequireComponent(typeof (ThirdPersonInputManager))]
[RequireComponent(typeof (PlayerClimbSystem))]
[RequireComponent(typeof (CharacterController))]
public class ThirdPersonPlayerController : PlayerController
{
    private InputManager playerInputManager;

    private CharacterController playerCharacterController;

    private ThirdPersonCameraController playerCamera;

    private Animator playerAnimator;

    [Header("Player Movement Attributes")]
    public PlayerMovementAttributes playerMovementAttributes;

    [Space(10)]
    public float playerCurrentSpeed;
    public float playerTargetSpeed;

    private float playerMoveSmoothVelocity = 0f;

    [Space(10)]
    public bool canPlayerMove = true;
    public bool canPlayerSprint = true;
    public bool isPlayerSprinting = false;

    private Vector2 directionVector;

    private Vector3 inputVector;
    private Vector3 desiredMove;

    [Header("Player Turning Attributes")]
    public PlayerTurningAttributes playerTurningAttributes;

    private float playerTurnSmoothVelocity;

    [Space(10)]
    public bool canPlayerTurn = true;

    [Header("Player Jumping Attributes")]
    public PlayerJumpingAttributes playerJumpingAttributes;

    [Space(10)]
    public Vector3 playerGravity = Physics.gravity;

    private Vector3 playerGravityVelocity = Vector3.zero;

    [Space(10)]
    public bool canPlayerJump = true;

    [Header("Player Rolling Attributes")]
    public PlayerRollingAttributes playerRollingAttributes;

    private RaycastHit rollHit;

    [Space(10)]
    public LayerMask rollMask;

    [Space(10)]
    public bool canPlayerRoll = true;
    public bool isPlayerRolling = false;

    [Header("Player Grounding Values")]
    public PlayerGroundingAttributes playerGroundingAttributes;

    private float playerHeight = 1f;

    private float playerCurrentSlopeAngle = 0f;

    private Vector3 currentGroundNormal = Vector3.up;

    [Space(10)]
    public LayerMask groundMask;

    private RaycastHit groundRayHit;

    private Vector3[] groundCheckVectors = new Vector3[4];

    [Space(10)]
    public bool isPlayerGrounded = true;
    public bool isPlayerOnSlope = false;

    private bool canPlayerCheckForGround = true;

    [Space(10)]
    public bool canPlayerCrouch = true;
    public bool isCrouched = false;

    [Header("Player Cover Attributes")]
    public PlayerCoverAttributes playerCoverAttributes;

    [Space(10)]
    public Cover playerCurrentCover = null;

    [Space(10)]
    public LayerMask coverMask;

    [Space(10)]
    public bool canPlayerTakeCover = true;
    public bool isPlayerInCover = false;

    [Header("Player Climbing Attributes")]
    public PlayerClimbingAttributes playerClimbingAttributes;

    private PlayerClimbSystem playerClimbSystem;

    [Space(10)]
    public LayerMask climbMask;

    [Space(10)]
    public bool canPlayerClimb = true;
    public bool isPlayerClimbing = false;

    [Header("Player Interaction Attributes")]
    public PlayerInteractionAttributes playerInteractionAttributes;

    [Space(10)]
    private GameObject[] interactableObjectsInRange;

    [Space(10)]
    public LayerMask interactableLayerMask;

    private RaycastHit interactRayHit;

    [Space(10)]
    public bool canPlayerInteract = true;

    [Header("Player Conversation Attributes")]
    public Conversation playerCurrentConversation = null;

    [Space(10)]
    public bool isPlayerInConversation = false;

    [Header("Player Camera Attributes")]
    public bool isUsingThirdPersonCam = true;

    [Header("Player Physics Interaction Attributes")]
    public PlayerPhysicsInteractionAttributes playerPhysicsInteractionAttributes;
    
    [Space(10)]
    public bool canPlayerInteractWithRigidbodies = true;

    [Header("Player Debug Attributes")]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializePlayer();
        InitializePlayerValues();
    }

    private void Update()
    {
        ManageGround();

        ManageTurning();

        ManageMovement();

        CheckForInteractableObjects();
    }

    #region PLAYER_INPUT

    public override void ReceiveInputVectors(Vector2 inputMovementVector, Vector2 inputHeadVector) //Input passthrough from Input Manager class
    {
        inputVector.Set(inputMovementVector.x, 0, inputMovementVector.y);

        if (inputVector.sqrMagnitude > 1)
        {
            inputVector.Normalize();
        }

        directionVector.Set(inputVector.x, inputVector.z);
    }

    #endregion

    #region PLAYER_INITIALIZE

    private void InitializePlayer()
    {
        playerInputManager = GetComponent<InputManager>();

        playerCharacterController = GetComponent<CharacterController>();

        playerClimbSystem = GetComponent<PlayerClimbSystem>();

        if (isUsingThirdPersonCam)
        {
            playerCamera = Camera.main.GetComponent<ThirdPersonCameraController>();
        }

        playerAnimator = GetComponentInChildren<Animator>();
    }

    private void InitializePlayerValues()
    {
        playerHeight = playerGroundingAttributes.playerBaseStandHeight;
        groundCheckVectors = ReturnGroundCheckVectors();
    }

    #endregion

    #region PLAYER_MOVEMENT

    private void ManageMovement()
    {
        if (canPlayerMove)
        {
            if (isPlayerInCover)
            {
                playerTargetSpeed = playerMovementAttributes.playerBaseCoverSpeed * inputVector.magnitude;
            }
            else if (isPlayerSprinting && isPlayerGrounded)
            {
                playerTargetSpeed = playerMovementAttributes.playerBaseSprintSpeed * inputVector.magnitude;
            }
            else
            {
                playerTargetSpeed = playerMovementAttributes.playerBaseWalkSpeed * inputVector.magnitude;
            }

            playerCurrentSpeed = Mathf.SmoothDamp(playerCurrentSpeed, playerTargetSpeed, ref playerMoveSmoothVelocity, GetSmoothTime());

            playerAnimator.SetFloat("moveSpeed", playerCurrentSpeed);

            if (isPlayerInCover)
            {
                PlayerCoverMovement();
            }
            else if (isPlayerGrounded)
            {
                PlayerGroundMovement();
            }
            else
            {
                PlayerAirMovement();
            }
        }
        else
        {
            playerCurrentSpeed = 0f;
            playerTargetSpeed = 0f;

            playerAnimator.SetFloat("moveSpeed", playerCurrentSpeed);
        }
    }

    private void PlayerGroundMovement()
    {
        if (playerCurrentSlopeAngle <= playerGroundingAttributes.playerMaxSlopeAngle)
        {
            desiredMove = ReturnPlayerForward() * playerCurrentSpeed;

            playerCharacterController.Move(desiredMove * Time.deltaTime);

            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, desiredMove, Color.blue);
            }
        }
    }

    private void PlayerAirMovement()
    {
        desiredMove = ReturnPlayerForward() * playerCurrentSpeed;

        desiredMove += playerGravityVelocity;

        playerCharacterController.Move(desiredMove * Time.deltaTime);

        if (canShowDebug)
        {
            Debug.DrawRay(transform.position, desiredMove, Color.yellow);
        }
    }

    private void PlayerCoverMovement()
    {
        int playerDirection = 0;

        if (directionVector.x < 0)
        {
            playerDirection = -1;
        }
        else if (directionVector.x > 0)
        {
            playerDirection = 1;
        }
        else if (directionVector.x == 0)
        {
            return;
        }

        Vector3 playerCoverSplineMovePosition = playerCurrentCover.ReturnCoverMovePosition(playerDirection, playerCurrentSpeed);

        transform.position = playerCoverSplineMovePosition;

        transform.LookAt(playerCoverSplineMovePosition);
    }

    private void ManageTurning()
    {
        if (canPlayerTurn)
        {
            if (directionVector != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;

                Vector3 newPlayerTurnRotation = new Vector3(transform.rotation.eulerAngles.x, 1 * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref playerTurnSmoothVelocity, playerTurningAttributes.playerTurnSmoothTime), transform.rotation.eulerAngles.z);

                transform.rotation = Quaternion.Euler(newPlayerTurnRotation);
            }
        }
    }

    public override void InitiateCrouch()
    {
        if (canPlayerCrouch)
        {
            if (isCrouched)
            {
                UnCrouch();
            }
            else
            {
                Crouch();
            }
        }
    }

    private void Crouch()
    {
        playerHeight = playerGroundingAttributes.playerCrouchHeight;

        playerCharacterController.height = playerGroundingAttributes.playerCrouchHeight;

        isCrouched = true;
    }

    private void UnCrouch()
    {
        playerHeight = playerGroundingAttributes.playerBaseStandHeight;

        playerCharacterController.height = playerGroundingAttributes.playerBaseStandHeight;

        isCrouched = false;
    }

    public override void Jump()
    {
        if (isPlayerInConversation)
        {
            playerCurrentConversation.MoveToNextConversationDialog();
        }
        else
        {
            if (canPlayerClimb)
            {
                if (isPlayerGrounded)
                {
                    if ((playerClimbSystem.TryClimb()))
                    {
                        playerClimbSystem.StartClimb();

                        return;
                    }
                }
            }

            if (canPlayerJump)
            {
                if (isPlayerGrounded)
                {
                    GroundJump();
                }
            }
        }     
    }

    private void GroundJump()
    {
        playerAnimator.SetTrigger("isJump");

        float newJumpVelocity = Mathf.Sqrt(-2 * playerGravity.y * playerJumpingAttributes.playerBaseJumpForce); //Kinematic Equation for calculating Jump Force

        Vector3 newJumpVector = Vector3.up * newJumpVelocity;

        playerGravityVelocity = newJumpVector;

        isPlayerGrounded = false;

        StartCoroutine(ResetGroundCheck());
    }

    public void PlayScaleAnimation()
    {
        playerAnimator.SetTrigger("isScale");
    }

    public void PlayVaultAnimation()
    {
        playerAnimator.SetTrigger("isVault");
    }

    public override void StartSprint()
    {
        if (canPlayerSprint)
        {
            isPlayerSprinting = true;
        }
    }

    public override void StopSprint()
    {
        isPlayerSprinting = false;
    }

    public void DisablePlayerMovement()
    {
        canPlayerMove = false;
        canPlayerTurn = false;
        canPlayerRoll = false;
        canPlayerSprint = false;
        canPlayerJump = false;
        canPlayerClimb = false;
        canPlayerInteract = false;
        canPlayerCrouch = false;
        canPlayerTakeCover = false;
    }

    public void EnablePlayerMovement()
    {
        canPlayerMove = true;
        canPlayerTurn = true;
        canPlayerRoll = true;
        canPlayerSprint = true;
        canPlayerJump = true;
        canPlayerClimb = true;
        canPlayerInteract = true;
        canPlayerCrouch = true;
        canPlayerTakeCover = true;
    }

    private float GetSmoothTime()
    {
        if (isPlayerInCover)
        {
            return playerMovementAttributes.playerCoverMoveSmoothTime;
        }
        else if (isPlayerGrounded)
        {
            if (isPlayerSprinting)
            {
                return playerMovementAttributes.playerSprintMoveSmoothTime;
            }
            else
            {
                return playerMovementAttributes.playerBaseMoveSmoothTime;
            }
        }
        else
        {
            if (playerMovementAttributes.playerAirControl == 0)
            {
                return float.MaxValue;
            }

            return playerMovementAttributes.playerBaseMoveSmoothTime / playerMovementAttributes.playerAirControl;
        }
    }

    #endregion;

    #region PLAYER_ROLLING

    private void PlayerSetupRoll()
    {
        if (canPlayerRoll)
        {
            Vector3 rollInputDirection = inputVector.normalized;

            if (rollInputDirection == Vector3.zero)
            {
                return;
            }

            Vector3 rollModifiedDirection = ReturnModifiedRollDirection(rollInputDirection);

            Vector3 rollStartPoint = transform.position;
            Vector3 rollEndPoint = ReturnRollEndPoint(rollModifiedDirection);

            float playerRollDistance = ReturnPlayerRollDistance(rollStartPoint, rollEndPoint); //Distance of Roll
            float playerAdjustedRollTime = ReturnPlayerRollTime(playerRollDistance); //Time to Complete Roll
            float playerRollSpeed = ReturnPlayerRollSpeed(playerRollDistance, playerAdjustedRollTime);

            if (rollEndPoint == Vector3.zero)
            {
                return;
            }

            transform.LookAt(rollEndPoint);

            playerAnimator.SetTrigger("isRoll");

            StartCoroutine(PlayerRoll(rollModifiedDirection, playerRollDistance, playerRollSpeed, playerAdjustedRollTime));
        }
    }

    private IEnumerator PlayerRoll(Vector3 rollModifiedDirection, float rollDistance, float rollSpeed, float rollTime)
    {
        canPlayerMove = false;
        canPlayerSprint = false;
        canPlayerTurn = false;
        canPlayerClimb = false;
        canPlayerCrouch = false;
        canPlayerJump = false;
        canPlayerRoll = false;
        canPlayerInteract = false;

        isPlayerRolling = true;

        float newRollTime = 0f;

        while (newRollTime < 1f)
        {
            newRollTime += Time.deltaTime / rollTime;

            float playerRollCurrentSpeed = rollSpeed * playerRollingAttributes.playerRollSpeedCurve.Evaluate(newRollTime);

            Vector3 rollVector = ReturnPlayerForward() * playerRollCurrentSpeed;
            rollVector += playerGravityVelocity;

            playerCharacterController.Move(rollVector * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        isPlayerRolling = false;

        canPlayerMove = true;
        canPlayerSprint = true;
        canPlayerTurn = true;
        canPlayerClimb = true;
        canPlayerCrouch = true;
        canPlayerJump = true;
        canPlayerRoll = true;
        canPlayerInteract = true;

        yield return null;
    }

    private Vector3 ReturnModifiedRollDirection(Vector3 baseRollDirection)
    {
        Vector3 modifiedRollDirection = playerCamera.transform.TransformDirection(baseRollDirection);
        baseRollDirection.y = 0;

        return modifiedRollDirection;
    }

    private Vector3 ReturnRollEndPoint(Vector3 modifiedRollDirection)
    {
        Vector3 rollStartPosition = transform.position;

        Vector3 rollEndPosition = rollStartPosition + (modifiedRollDirection * playerRollingAttributes.playerMaxRollingDistance);
        rollEndPosition.y = rollStartPosition.y;

        if (canShowDebug)
        {
            Debug.DrawLine(rollStartPosition, rollEndPosition, Color.green, 1f);
        }

        if (Physics.Raycast(rollStartPosition, modifiedRollDirection, out rollHit, playerRollingAttributes.playerMaxRollingDistance, rollMask))
        {           
            float pointDistance = Vector3.Distance(rollStartPosition, rollEndPosition);

            if (pointDistance < playerRollingAttributes.playerMinRollingDistance)
            {
                return Vector3.zero;
            }

            rollEndPosition = rollHit.point;
            rollEndPosition.y = rollStartPosition.y;

            if (canShowDebug)
            {
                Debug.DrawLine(rollStartPosition, rollEndPosition, Color.red, 1f);
            }
        }

        return rollEndPosition;
    }

    private float ReturnPlayerRollDistance(Vector3 rollStart, Vector3 rollEnd)
    {
        float rollDistance = Vector3.Distance(rollStart, rollEnd);

        return rollDistance;
    }

    private float ReturnPlayerRollSpeed(float rollDistance, float rollTime)
    {
        float newRollSpeed = rollDistance / rollTime;

        return newRollSpeed;
    }

    private float ReturnPlayerRollTime(float rollDistance)
    {
        float newRollTime = (rollDistance / playerRollingAttributes.playerMaxRollingDistance) * playerRollingAttributes.playerBaseRollTime;

        return newRollTime;
    }

    #endregion

    #region PLAYER_WALLSCALING

    public void DisableMovementBeforeClimb()
    {
        canPlayerMove = false;
        canPlayerSprint = false;
        canPlayerTurn = false;
        canPlayerJump = false;

        canPlayerCheckForGround = false;

        playerCurrentSpeed = 0;
    }

    public void EnableMovementAfterClimb()
    {
        canPlayerMove = true;
        canPlayerSprint = true;
        canPlayerTurn = true;
        canPlayerJump = true;

        canPlayerCheckForGround = true;
    }

    #endregion

    #region PLAYER_GROUND

    private void ManageGround()
    {
        GroundCheck();

        ManagePlayerGravity();

        SlopeCheck();

        CalculateGroundAngle();

        ManageSlopeRotation();
    }

    private void GroundCheck()
    {
        if (canPlayerCheckForGround)
        {
            for (int i = 0; i < groundCheckVectors.Length; i++)
            {
                if (Physics.Raycast(transform.position + groundCheckVectors[i], Vector3.down, out groundRayHit, playerHeight + playerGroundingAttributes.playerHeightPadding, groundMask))
                {
                    playerGravityVelocity = Vector3.zero;

                    isPlayerGrounded = true;

                    if (canShowDebug)
                    {
                        Debug.DrawRay(transform.position + groundCheckVectors[i], Vector3.down, Color.green);
                    }
                }
                else
                {
                    isPlayerGrounded = false;

                    currentGroundNormal = Vector3.up;

                    if (canShowDebug)
                    {
                        Debug.DrawRay(transform.position + groundCheckVectors[i], Vector3.down, Color.red);
                    }

                    break;
                }
            }
        }
    }

    private IEnumerator ResetGroundCheck()
    {
        canPlayerCheckForGround = false;

        yield return new WaitForSeconds(0.1f);

        canPlayerCheckForGround = true;

        yield return null;
    }

    private void ManagePlayerGravity()
    {
        if (!isPlayerGrounded)
        {
            playerGravityVelocity += (Time.deltaTime * playerGravity);
        }
        else
        {
            playerGravityVelocity = Vector3.zero;
        }
    }

    private void SlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groundRayHit, playerGroundingAttributes.playerSlopeDetectionRayLength + playerGroundingAttributes.playerHeightPadding, groundMask))
        {
            currentGroundNormal = groundRayHit.normal;

            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, Vector3.down * (playerGroundingAttributes.playerSlopeDetectionRayLength + playerGroundingAttributes.playerHeightPadding), Color.yellow);
            }
        }
        else
        {
            currentGroundNormal = Vector3.up;

            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, Vector3.down * (playerGroundingAttributes.playerSlopeDetectionRayLength + playerGroundingAttributes.playerHeightPadding), Color.black);
            }
        }
    }

    private void CalculateGroundAngle()
    {
        if (!isPlayerGrounded)
        {
            playerCurrentSlopeAngle = 90;

            isPlayerOnSlope = false;

            return;
        }

        playerCurrentSlopeAngle = Vector3.Angle(currentGroundNormal, transform.forward);

        if (currentGroundNormal != Vector3.up)
        {
            isPlayerOnSlope = true;
        }
        else
        {
            isPlayerOnSlope = false;
        }
    }

    private void ManageSlopeRotation()
    {
        Quaternion newPlayerSlopeRotation = Quaternion.LookRotation(ReturnPlayerForward());

        transform.rotation = Quaternion.Slerp(transform.rotation, newPlayerSlopeRotation, Time.deltaTime * playerGroundingAttributes.playerSlopeAdjustSmoothTime);
    }

    private Vector3 ReturnPlayerForward()
    {
        return Vector3.Cross(transform.right, currentGroundNormal);
    }

    private Vector3[] ReturnGroundCheckVectors()
    {
        Vector3[] newGroundCheckVectors = new Vector3[4];

        newGroundCheckVectors[0] = new Vector3(-playerCharacterController.radius / 4, 0f, -playerCharacterController.radius / 4);
        newGroundCheckVectors[1] = new Vector3(playerCharacterController.radius / 4, 0f, -playerCharacterController.radius / 4);
        newGroundCheckVectors[2] = new Vector3(-playerCharacterController.radius / 4, 0f, playerCharacterController.radius / 4);
        newGroundCheckVectors[3] = new Vector3(playerCharacterController.radius / 4, 0f, playerCharacterController.radius / 4);

        return newGroundCheckVectors;
    }

    #endregion

    #region PLAYER_COVER

    public override void TakeCover()
    {
        if (CheckForCoverInPlayerRadius())
        {
            PlayerTakeCover();
        }
        else
        {
            PlayerSetupRoll();
        }
    }

    private void PlayerTakeCover()
    {
        if (canPlayerTakeCover)
        {
            if (!isPlayerInCover)
            {
                if (CheckForCoverInPlayerRadius())
                {
                    Cover newPlayerCover = ReturnPlayerCover();

                    CoverPoint newPlayerCoverPoint = newPlayerCover.ReturnClosestCoverPoint(transform.position);

                    float distanceToCoverPoint = Vector3.Distance(transform.position, newPlayerCoverPoint.transform.position);

                    if (CheckPlayerPathToCover(newPlayerCoverPoint, distanceToCoverPoint))
                    {
                        StartCoroutine(TransitionPlayerToCover(newPlayerCover, newPlayerCoverPoint, distanceToCoverPoint));
                    }
                }
            }
            else
            {
                PlayerLeaveCover();
            }
        }
    }

    private IEnumerator TransitionPlayerToCover(Cover targetCover, CoverPoint targetCoverPoint, float distanceToCover)
    {
        float coverSplinePercent = 1f / targetCover.coverPoints.Length;
        coverSplinePercent *= targetCoverPoint.coverPointIndex + 1;

        Vector3 targetCoverHookLocation = targetCover.coverSpline.GetPoint(coverSplinePercent);

        transform.LookAt(targetCoverHookLocation);

        playerAnimator.SetTrigger("isSlide");

        canPlayerMove = false;
        canPlayerTurn = false;
        canPlayerTakeCover = false;

        Vector3 playerStartPosition = transform.position;

        float speedMultiplier = playerCoverAttributes.playerCoverDetectionRadius / distanceToCover;

        float currentCoverTransitionTime = 0;

        while(currentCoverTransitionTime < 1)
        {
            currentCoverTransitionTime += (Time.deltaTime * speedMultiplier) / playerCoverAttributes.playerCoverTransitionTime;

            if (currentCoverTransitionTime > 1)
            {
                currentCoverTransitionTime = 1;
            }

            transform.position = Vector3.Lerp(playerStartPosition, targetCoverHookLocation, currentCoverTransitionTime);

            yield return new WaitForEndOfFrame();
        }

        FinishPlayerMoveToCover(targetCover, targetCoverPoint, targetCoverHookLocation, coverSplinePercent);
        
        yield return null;
    }

    private void FinishPlayerMoveToCover(Cover targetCover, CoverPoint targetCoverPoint, Vector3 coverSplinePercentPosition, float targetCoverSplinePercent)
    {
        transform.position = coverSplinePercentPosition;

        canPlayerMove = true;
        canPlayerTakeCover = true;
        isPlayerInCover = true;

        playerCurrentCover = targetCover;
        playerCurrentCover.isPlayerAttachedToCover = true;
        playerCurrentCover.playerAttachedCoverPointIndex = targetCoverPoint.coverPointIndex;
        playerCurrentCover.playerCoverSplineMovePercent = targetCoverSplinePercent;

        playerCamera.currentLockOnTarget = playerCurrentCover.transform;
        playerCamera.isPlayerInCover = true;
    }

    private void PlayerLeaveCover()
    {
        isPlayerInCover = false;

        playerCamera.isPlayerInCover = false;
        playerCamera.currentLockOnTarget = null;

        canPlayerMove = true;
        canPlayerTurn = true;

        playerCurrentCover.ResetCover();

        playerCurrentCover = null;
    }

    private bool CheckForCoverInPlayerRadius()
    {
        Collider[] hitCoverColliders = Physics.OverlapSphere(transform.position, playerCoverAttributes.playerCoverDetectionRadius, coverMask);

        if (hitCoverColliders.Length > 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckPlayerPathToCover(CoverPoint coverPointToCheck, float distanceToCover)
    {
        float verticalDistanceBetweenCover = Mathf.Abs(coverPointToCheck.transform.position.y - transform.position.y);

        if (verticalDistanceBetweenCover > 0.5f) //Check if Cover is on similar Y coordinates
        {
            return false;
        }

        Vector3 interceptVector = (coverPointToCheck.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, interceptVector, distanceToCover))
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * distanceToCover, Color.red, 1f);
            }

            return false;
        }
        else
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * distanceToCover, Color.green, 1f);
            }

            return true;
        }
    }

    private Cover ReturnPlayerCover()
    {
        Cover newPlayerCover = null;

        float closestCoverPoint = float.MaxValue;

        Collider[] hitCoverColliders = Physics.OverlapSphere(transform.position, playerCoverAttributes.playerCoverDetectionRadius, coverMask);

        for (int i = 0; i < hitCoverColliders.Length; i++)
        {
            Cover coverToCheck = hitCoverColliders[i].GetComponent<Cover>();

            float coverDistanceToPlayer = Vector3.Distance(transform.position, coverToCheck.transform.position);

            if (coverDistanceToPlayer < closestCoverPoint)
            {
                closestCoverPoint = coverDistanceToPlayer;

                newPlayerCover = coverToCheck;
            }
        }

        return newPlayerCover;
    }

    #endregion

    #region PLAYER_INTERACTION

    public override void Interact()
    {
        PlayerInteraction();
    }

    private void PlayerInteraction()
    {
        if (canPlayerInteract)
        {
            if (interactableObjectsInRange.Length > 0)
            {
                Interactable newInteractableObject = ReturnClosestInteractableObject();

                if (CheckForInteractAngle(newInteractableObject))
                {
                    float distanceFromPlayer = Vector3.Distance(transform.position, newInteractableObject.transform.position);

                    if (CheckPathToInteractableObject(newInteractableObject, distanceFromPlayer))
                    {
                        newInteractableObject.Interact(gameObject);
                    }
                    else
                    {
                        Debug.Log("No Path to Object Interaction :: " + newInteractableObject);
                    }
                }         
            }
        }
    }

    private Interactable ReturnClosestInteractableObject()
    {
        Interactable closestInteractableObject = null;

        float closestInteractableObjectDistance = float.MaxValue;

        for (int i = 0; i < interactableObjectsInRange.Length; i++)
        {
            Interactable newInteractable = interactableObjectsInRange[i].GetComponent<Interactable>();

            float interactableObjectDistance = Vector3.Distance(transform.position, newInteractable.transform.position);

            if (interactableObjectDistance < closestInteractableObjectDistance)
            {
                closestInteractableObjectDistance = interactableObjectDistance;

                closestInteractableObject = newInteractable;
            }
        }

        return closestInteractableObject;
    }

    private bool CheckPathToInteractableObject(Interactable interactableObject, float playerDistanceFromInteractable)
    {
        float verticalDistanceBetweenInteraction = Mathf.Abs(interactableObject.transform.position.y - transform.position.y);

        if (verticalDistanceBetweenInteraction > playerInteractionAttributes.playerInteractionMaxVerticalHeight) //Check if Cover is on similar Y coordinates
        {
            Debug.Log("Object is too high :: " + verticalDistanceBetweenInteraction);

            return false;
        }

        Vector3 interceptVector = (interactableObject.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, interceptVector, out interactRayHit, playerDistanceFromInteractable))
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * playerDistanceFromInteractable, Color.red, 1f);
            }

            if (interactRayHit.collider.gameObject != interactableObject.gameObject)
            {
                Debug.Log(interactRayHit.collider.gameObject.name);

                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * playerDistanceFromInteractable, Color.green, 1f);
            }

            return true;
        }
    }

    private bool CheckForInteractAngle(Interactable interactableObject)
    {
        if (interactableObject.interactableObjectType == INTERACTABLE_OBJECT_TYPE.ACTOR)
        {
            Vector3 interceptVector = interactableObject.transform.position - transform.position;

            float vectorAngle = Vector3.Angle(interceptVector, transform.forward);

            if (vectorAngle > playerInteractionAttributes.playerInteractionActorMaxAngle)
            {
                Debug.Log(vectorAngle);

                return false;
            }
        }
        else if (interactableObject.interactableObjectType == INTERACTABLE_OBJECT_TYPE.PHYSICS)
        {
            Vector3 interceptVector = interactableObject.transform.position - transform.position;

            float vectorAngle = Vector3.Angle(interceptVector, transform.forward);

            if (vectorAngle > playerInteractionAttributes.playerInteractionPhysicsMaxAngle)
            {
                return false;
            }
        }

        return true;
    }

    private void CheckForInteractableObjects()
    {
        if (canPlayerInteract)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, playerInteractionAttributes.playerInteractionMaxRadius, interactableLayerMask);
            interactableObjectsInRange = new GameObject[objectsInRange.Length];

            for (int i = 0; i < objectsInRange.Length; i++)
            {
                interactableObjectsInRange[i] = objectsInRange[i].gameObject;
            }
        }
    }

    #endregion

    #region PLAYER_CONVERSATION

    public void PlayerEnterConversation(Conversation newConversation)
    {
        isPlayerInConversation = true;
        playerCurrentConversation = newConversation;

        canPlayerMove = false;
        canPlayerSprint = false;
        canPlayerTurn = false;
        canPlayerJump = false;
        canPlayerTakeCover = false;
        canPlayerClimb = false;
        canPlayerCheckForGround = false;

        playerCurrentSpeed = 0;
    }

    public void PlayerEndConversation()
    {
        isPlayerInConversation = false;
        playerCurrentConversation = null;

        canPlayerMove = true;
        canPlayerSprint = true;
        canPlayerTurn = true;
        canPlayerJump = true;
        canPlayerTakeCover = true;
        canPlayerClimb = true;
        canPlayerCheckForGround = true;
    }

    #endregion

    #region EDITOR_DEBUG
    private void OnDrawGizmosSelected()
    {
        if (canShowDebug)
        {
            //Draw Cover Radius
            Gizmos.color = Colors.AliceBlue;
            Gizmos.DrawWireSphere(transform.position, playerCoverAttributes.playerCoverDetectionRadius);

            //Draw Interact Radius
            Gizmos.color = Colors.ForestGreen;
            Gizmos.DrawWireSphere(transform.position, playerInteractionAttributes.playerInteractionMaxRadius);
        }
    }
    #endregion
}

[System.Serializable]
public class PlayerMovementAttributes
{
    [Header("Player Movement Attributes")]
    public float playerBaseWalkSpeed = 1f;
    public float playerBaseSprintSpeed = 1f;
    public float playerBaseCoverSpeed = 1f;

    [Space(10)]
    public float playerBaseMoveSmoothTime = 0f;
    public float playerSprintMoveSmoothTime = 0f;
    public float playerCoverMoveSmoothTime = 0f;

    [Space(10)]
    [Range(0, 1f)]
    public float playerAirControl = 0f;

    public PlayerMovementAttributes(float playerBaseWalkSpeed, float playerBaseSprintSpeed, float playerBaseCoverSpeed, float playerBaseMoveSmoothTime, float playerSprintMoveSmoothTime, float playerCoverMoveSmoothTime, float playerAirControl, float playerBaseStandingHeight, float playerCrouchHeight, float playerHeightPadding, float playerTurnSmoothTime, float playerBaseJumpForce, float playerTerminalVelocity)
    {
        this.playerBaseWalkSpeed = playerBaseWalkSpeed;
        this.playerBaseSprintSpeed = playerBaseSprintSpeed;
        this.playerBaseCoverSpeed = playerBaseCoverSpeed;
        this.playerBaseMoveSmoothTime = playerBaseMoveSmoothTime;
        this.playerSprintMoveSmoothTime = playerSprintMoveSmoothTime;
        this.playerCoverMoveSmoothTime = playerCoverMoveSmoothTime;
        this.playerAirControl = playerAirControl;
    }
}

[System.Serializable]
public class PlayerTurningAttributes
{
    [Header("Player Turning Attributes")]
    public float playerTurnSmoothTime = 0.2f;

    public PlayerTurningAttributes (float playerTurnSmoothTime)
    {
        this.playerTurnSmoothTime = playerTurnSmoothTime;
    }
}

[System.Serializable]
public class PlayerJumpingAttributes
{
    [Header("Player Jumping Attributes")]
    public float playerBaseJumpForce = 1f;

    [Space(10)]
    public float playerTerminalVelocity = -50f;

    public PlayerJumpingAttributes (float playerBaseJumpForce, float playerTerminalVelocity)
    {
        this.playerBaseJumpForce = playerBaseJumpForce;
        this.playerTerminalVelocity = playerTerminalVelocity;
    }
}

[System.Serializable]
public class PlayerClimbingAttributes
{
    [Header("Player Climbing Attributes")]
    public float playerClimbRayLength = 1f;

    [Header("Player Wall Scaling Attributes")]
    public float playerScalingMinHorizontal = 1f;
    public float playerScalingMaxHorizontal = 1f;

    [Space(10)]
    public float playerScalingMinVertical = 1f;
    public float playerScalingMaxVertical = 1f;

    [Space(10)]
    public float playerWallScaleTime = 1f;

    [Header("Player Wall Vaulting Attributes")]
    public float playerVaultingHorizontalMin = 1f;
    public float playerVaultingHorizontalMax = 3f;

    [Space(10)]
    public float playerVaultingVerticalMin = 1f;
    public float playerVaultingVerticalMax = 3f;

    [Space(10)]
    public float playerWallVaultingTime = 1f;

    public PlayerClimbingAttributes (float playerClimbRayDistance, float playerScalingMinHorizontal, float playerScalingMaxHorizontal, float playerScalingMinVertical, float playerScalingMaxVertical, float playerWallScaleTime, float playerWallVaultingHorizontalMinDistance, float playerWallVaultingHorizontalMaxDistance, float playerWallVaultingVerticalMinDistance, float playerWallVaultingVerticalMaxDistance, float playerWallVaultingTime)
    {
        this.playerClimbRayLength = playerClimbRayDistance;
        this.playerScalingMinHorizontal = playerScalingMinHorizontal;
        this.playerScalingMaxHorizontal = playerScalingMaxHorizontal;
        this.playerScalingMinVertical = playerScalingMinVertical;
        this.playerScalingMaxVertical = playerScalingMaxVertical;
        this.playerWallScaleTime = playerWallScaleTime;
        this.playerVaultingHorizontalMin = playerWallVaultingHorizontalMinDistance;
        this.playerVaultingHorizontalMax = playerWallVaultingHorizontalMaxDistance;
        this.playerVaultingVerticalMin = playerWallVaultingVerticalMinDistance;
        this.playerVaultingVerticalMax = playerWallVaultingVerticalMaxDistance;
        this.playerWallVaultingTime = playerWallVaultingTime;
    }
}

[System.Serializable]
public class PlayerRollingAttributes
{
    [Header("Player Roll Attributes")]
    public float playerMinRollingDistance = 1f;
    public float playerMaxRollingDistance = 3f;

    [Space(10)]
    public float playerBaseRollTime;

    [Space(10)]
    public AnimationCurve playerRollSpeedCurve;

    public PlayerRollingAttributes(float playerMinRollDistance, float playerMaxRollDistance, float playerRollSpeed)
    {
        this.playerMinRollingDistance = playerMinRollDistance;
        this.playerMaxRollingDistance = playerMaxRollDistance;
        this.playerBaseRollTime = playerRollSpeed;
    }
}

[System.Serializable]
public class PlayerGroundingAttributes
{
    [Header("Player Height Attributes")]
    public float playerBaseStandHeight = 1.5f;
    public float playerCrouchHeight = 1f;

    [Space(10)]
    public float playerHeightPadding = 0.2f;

    [Header("Player Slope Attributes")]
    public float playerSlopeDetectionRayLength = 1f;

    [Space(10)]
    public float playerMaxSlopeAngle = 120f;
    public float playerSlopeAdjustSmoothTime = 5f;

    public PlayerGroundingAttributes (float playerBaseStandHeight, float playerCrouchHeight, float playerHeightPadding, float playerSlopeDetectionRayLength, float playerMaxSlopeAngle, float playerSlopeAdjustmentTime)
    {
        this.playerBaseStandHeight = playerBaseStandHeight;
        this.playerCrouchHeight = playerCrouchHeight;
        this.playerHeightPadding = playerHeightPadding;
        this.playerSlopeDetectionRayLength = playerSlopeDetectionRayLength;
        this.playerMaxSlopeAngle = playerMaxSlopeAngle;
        this.playerSlopeAdjustSmoothTime = playerSlopeAdjustmentTime;
    }
}

[System.Serializable]
public class PlayerCoverAttributes
{
    [Header("Player Cover Attributes")]
    public float playerCoverDetectionRadius = 5f;

    [Space(10)]
    public float playerCoverTransitionTime = 1f;

    public PlayerCoverAttributes (float playerCoverDetectionRadius, float playerCoverTransitionTime)
    {
        this.playerCoverDetectionRadius = playerCoverDetectionRadius;
        this.playerCoverTransitionTime = playerCoverTransitionTime;
    }
}

[System.Serializable]
public class PlayerInteractionAttributes
{
    [Header("Player Interaction Attributes")]
    public float playerInteractionMaxRadius = 1f;
    public float playerInteractionMaxVerticalHeight = 1f;
    public float playerInteractionPhysicsMaxAngle = 60f;
    public float playerInteractionActorMaxAngle = 30f;

    public PlayerInteractionAttributes(float playerInteractionMaxRadius, float playerInteractionMaxVerticalHeight, float playerInteractionPhysicsMaxAngle, float playerInteractionActorMaxAngle)
    {
        this.playerInteractionMaxRadius = playerInteractionMaxRadius;
        this.playerInteractionMaxVerticalHeight = playerInteractionMaxVerticalHeight;
        this.playerInteractionPhysicsMaxAngle = playerInteractionPhysicsMaxAngle;
        this.playerInteractionActorMaxAngle = playerInteractionActorMaxAngle;
    }
}

[System.Serializable]
public class PlayerPhysicsInteractionAttributes
{
    [Header("Player Physics Interaction Attributes")]
    public float playerPhysicsCollisionMultiplier = 1f;

    public PlayerPhysicsInteractionAttributes (float playerPhysicsCollisionMultiplier)
    {
        this.playerPhysicsCollisionMultiplier = playerPhysicsCollisionMultiplier;
    }
}