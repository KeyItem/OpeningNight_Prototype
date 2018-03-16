using System.Collections;
using UnityEngine;

public class ActorAnimationController : MonoBehaviour
{
    [Header("Actor Animation Controller")]
    public Animator actorAnimator;

    private void Start()
    {
        ActorAnimatorControllerSetup();
    }

    private void ActorAnimatorControllerSetup()
    {
        actorAnimator = GetComponentInChildren<Animator>();
    }

    public void ReceiveNewAnimationData(ActorAnimationData newActorAnimationData)
    {

    }
}
