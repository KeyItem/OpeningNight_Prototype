using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Conversation Data", menuName = "Conversation/Coversation Data", order = 1201)]
public class ConversationData : ScriptableObject
{
    [Header("Conversation Dialogue Attributes")]
    public string[] conversationDialogue;

    [Header("Coversation Speaker Attributes")]
    public string[] conversationSpeakerName;

    [Header("Conversation Camera Actions")]
    public ConversationCameraEvent[] conversationCameraEvents;
}
