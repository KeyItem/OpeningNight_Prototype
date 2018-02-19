using System.Collections;
using UnityEngine;

public class ActorInteractable : Interactable
{
    [Header("Actor Interaction Attributes")]
    public Conversation actorConversation;

    [Space(10)]
    public bool isInConversation = false;

    public override void Interact(GameObject playerInteracting)
    {
        actorConversation.ImportConversation(actorConversation.currentConversationData);
    }
}
