using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ActorNavigationController : MonoBehaviour
{
    private ActorController targetActorController;

    private NavMeshAgent navAgent;

    private Animator actorAnimator;

    [Header("Actor Navigation Attributes")]
    public ActorMovementInfo currentActorMovementInfo;

    public int currentActorMovementInfoIndex;

    [Header("Actor Current Movement Data Attributes")]
    public ActorMovementData currentActorMovementData;

    [Space(10)]
    public REPEAT_TYPE currentMovementRepeatType;

    [Space(10)]
    public Transform actorCurrentTargetNavPoint;

    [Space(10)]
    public int currentMovementDataIndex = 0;

    [Space(10)]
    public float actorCurrentMoveSpeed = 1f;

    [Space(10)]
    public bool canActorMove = true;

    [Space(10)]
    public bool isActorMoving = false;

    private void Start()
    {
        ActorNavigationSetup();
    }

    private void Update()
    {
        ManageNavigation();
    }

    private void ActorNavigationSetup()
    {
        targetActorController = GetComponent<ActorController>();

        navAgent = GetComponent<NavMeshAgent>();
        actorAnimator = GetComponentInChildren<Animator>();
    }

    public void ImportNewActorMovementInfo(ActorMovementInfo newActorMovementInfo, REPEAT_TYPE movementRepeatType)
    {
        currentActorMovementInfoIndex = 0;
        currentMovementDataIndex = 0;

        currentActorMovementInfo = newActorMovementInfo;

        currentMovementRepeatType = movementRepeatType;

        ImportNewActorMovementData(newActorMovementInfo.actorMovement[0]);
    }

    private void ImportNewActorMovementData(ActorMovementData newActorMovementData)
    {
        currentMovementDataIndex = 0;

        currentActorMovementData = newActorMovementData;

        SetNewNavigationTarget();
    }

    private void ManageNavigation()
    {
        if (canActorMove)
        {
            if (isActorMoving)
            {
                if (!navAgent.pathPending)
                {
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                    {
                        if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                        {
                            ReachedDestination();
                        }
                    }
                }
            }
        }
    }

    private void ReachedDestination()
    {
        ManageNextMovementAction();
    }

    private void SetNewNavigationTarget()
    {
        actorCurrentMoveSpeed = currentActorMovementData.actorMovePointSpeeds[currentMovementDataIndex];
        actorCurrentTargetNavPoint = currentActorMovementData.actorMovePointTransform[currentMovementDataIndex];

        Debug.Log(actorCurrentTargetNavPoint.position);

        navAgent.speed = ReturnModifiedMovementTime(transform.position, actorCurrentTargetNavPoint.position, actorCurrentMoveSpeed);

        navAgent.updatePosition = true;
        navAgent.updateRotation = true;

        navAgent.SetDestination(actorCurrentTargetNavPoint.position);

        actorAnimator.SetBool("isMoving", true);

        isActorMoving = true;

        navAgent.isStopped = false;
    }

    private void FinishedAllMovementActions()
    {
        actorAnimator.SetBool("isMoving", false);

        isActorMoving = false;

        navAgent.isStopped = true;

        navAgent.updatePosition = false;
        navAgent.updateRotation = false;

        transform.rotation = actorCurrentTargetNavPoint.rotation;

        actorCurrentTargetNavPoint = null;
    }

    private float ReturnModifiedMovementTime(Vector3 currentPosition, Vector3 targetPosition, float moveToTime)
    {
        float targetDistance = Vector3.Distance(currentPosition, targetPosition);

        if (targetDistance < 1)
        {
            return 1f;
        }

        float modifiedMoveTime = targetDistance / moveToTime;

        return modifiedMoveTime;
    }

    private void ManageNextMovementAction()
    {
        if (CanMoveToNextWaypoint())
        {
            SetNewNavigationTarget();
        }
        else if (CanMoveToNextMovementData())
        {
            ImportNewActorMovementData(currentActorMovementInfo.actorMovement[currentActorMovementInfoIndex]);
        }
        else
        {
            if (currentMovementRepeatType != REPEAT_TYPE.NONE)
            {
                if (currentMovementRepeatType == REPEAT_TYPE.CYCLIC)
                {
                    RepeatCyclic();
                }
                else if (currentMovementRepeatType == REPEAT_TYPE.REVERSE)
                {
                    RepeatReverse();
                }
            }
            else
            {
                FinishedAllMovementActions();
            }
        }
    }

    private void RepeatCyclic()
    {
        Debug.Log("Repeat Cyclic");

        currentActorMovementInfoIndex = 0;
        currentMovementDataIndex = 0;

        SetNewNavigationTarget();
    }

    private void RepeatReverse()
    {
        Debug.Log("Repeat Reverse");

        for (int i = 0; i < currentActorMovementInfo.actorMovement.Length; i++)
        {
            System.Array.Reverse(currentActorMovementInfo.actorMovement[i].actorMovePointTransform);
        }

        currentActorMovementInfoIndex = 0;
        currentMovementDataIndex = 0;

        SetNewNavigationTarget();
    }

    private bool CanMoveToNextWaypoint()
    {
        int movementIndex = currentMovementDataIndex;

        if (++movementIndex <= currentActorMovementData.actorMovePointTransform.Length - 1)
        {
            currentMovementDataIndex++;

            return true;
        }

        return false;
    }

    private bool CanMoveToNextMovementData()
    {
        int movementDataIndex = currentActorMovementInfoIndex;

        if (++movementDataIndex <= currentActorMovementInfo.actorMovement.Length - 1)
        {
            currentActorMovementInfoIndex++;

            return true;
        }

        return false;
    }
}
