using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ActorNavigationController : MonoBehaviour
{
    private NavMeshAgent navAgent;

    private Animator actorAnimator;

    [Header("Actor Navigation Attributes")]
    public ActorMovementData currentActorMovementData;

    [Space(10)]
    public Transform actorCurrentTargetNavPoint;

    [Space(10)]
    public int currentMovementIndex = 0;

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
        navAgent = GetComponent<NavMeshAgent>();
        actorAnimator = GetComponentInChildren<Animator>();
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
        if (currentActorMovementData.actorMovePointTransform.Length > 0)
        {
            if (CanMoveToNextMovementAction())
            {
                SetNewNavigationTarget();
            }
            else
            {
                FinishedRoute();
            }
        }
    }

    private void SetNewNavigationTarget()
    {
        actorCurrentMoveSpeed = currentActorMovementData.actorMovePointSpeeds[currentMovementIndex];
        actorCurrentTargetNavPoint = currentActorMovementData.actorMovePointTransform[currentMovementIndex];

        navAgent.speed = actorCurrentMoveSpeed;

        navAgent.updatePosition = true;
        navAgent.updateRotation = true;

        navAgent.SetDestination(actorCurrentTargetNavPoint.position);

        actorAnimator.SetBool("isMoving", true);

        isActorMoving = true;

        navAgent.isStopped = false;
    }

    private void FinishedRoute()
    {
        actorAnimator.SetBool("isMoving", false);

        isActorMoving = false;

        navAgent.isStopped = true;

        navAgent.updatePosition = false;
        navAgent.updateRotation = false;

        transform.rotation = actorCurrentTargetNavPoint.rotation;

        actorCurrentTargetNavPoint = null;
    }

    public void ReceiveNewActorMovementData(ActorMovementData newActorMovementData)
    {
        currentMovementIndex = 0;

        currentActorMovementData = newActorMovementData;

        SetNewNavigationTarget();
    }

    public bool CanMoveToNextMovementAction()
    {
        int movementIndex = currentMovementIndex;

        if (++movementIndex <= currentActorMovementData.actorMovePointTransform.Length - 1)
        {
            currentMovementIndex++;

            return true;
        }

        return false;
    }
}
