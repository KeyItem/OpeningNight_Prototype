using System.Collections;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    private ThirdPersonPlayerController playerController;

    private ConversationDialogDisplay conversationDialogDisplay;
    private ConversationCamera conversationCamera;

    [Header("Conversation Attributes")]
    public ConversationData currentConversationData;

    [Space(10)]
    public int currentConversationIndex = 0;
    public int maxConversationIndex = 0;

    [Space(10)]
    public bool isConversationActive = false;

    [Header("Conversation Camera Position")]
    public Transform[] conversationCameraTransforms;

    public void ImportConversation(ConversationData newConversationData)
    {
        currentConversationData = newConversationData;

        currentConversationIndex = 0;
        maxConversationIndex = newConversationData.conversationDialogue.Length;

        PrepareConversation();
    }

    private void PrepareConversation()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonPlayerController>();

        conversationCamera = GameObject.FindGameObjectWithTag("ConversationCamera").GetComponent<ConversationCamera>();
        conversationDialogDisplay = GameObject.FindGameObjectWithTag("ConversationUI").GetComponent<ConversationDialogDisplay>();

        conversationCamera.ImportConversation(this);
        conversationCamera.ImportCoversationData(currentConversationData);

        conversationDialogDisplay.ImportConversationData(currentConversationData);

        StartConversation();
    }

    private void StartConversation()
    {
        isConversationActive = true;

        playerController.PlayerEnterConversation(this);

        conversationCamera.StartConversation();
        conversationDialogDisplay.StartConversation();

        StartConversationDialog();
    }

    private void EndConversation()
    {
        isConversationActive = false;

        playerController.PlayerEndConversation();

        conversationCamera.EndConversation();
        conversationDialogDisplay.EndConversation();
    }

    private void StartConversationDialog()
    {
        conversationDialogDisplay.ReadConversationDialog(0);
        conversationCamera.StartCameraEffect(0);
    }

    public void MoveToNextConversationDialog()
    {
        if (isConversationActive)
        {
            if (CanMoveToNextDialog())
            {
                conversationDialogDisplay.ReadConversationDialog(currentConversationIndex);
                conversationCamera.StartCameraEffect(currentConversationIndex);
            }
            else
            {
                EndConversation();
            }
        }
    }

    private bool CanMoveToNextDialog()
    {
        if (++currentConversationIndex < maxConversationIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
