using System.Collections;
using UnityEngine;

public class ActorAnimationController : MonoBehaviour
{
    [Header("Actor Animation Controller")]
    public Animator actorAnimator;
    private AnimatorOverrideController actorAnimatorOverrideController;

    private void Start()
    {
        ActorAnimatorControllerSetup();
    }

    private void ActorAnimatorControllerSetup()
    {
        actorAnimator = GetComponentInChildren<Animator>();
    }

    public void ImportNewActorAnimationInfo(ActorAnimationInfo newActorAnimationInfo, REPEAT_TYPE repeatType)
    {
        
    }

    private void ManageActorAnimations()
    {
        //actorAnimatorOverrideController["Action1"] = newActorAnimationInfo.actorAnimations;
    }
}
