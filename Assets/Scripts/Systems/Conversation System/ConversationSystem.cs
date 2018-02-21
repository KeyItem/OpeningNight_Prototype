﻿using System.Collections;
using UnityEngine;

public class ConversationSystem : MonoBehaviour
{
    public static ConversationSystem Instance;

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

    private void Awake()
    {
        InitializeConversationSystem();
    }

    private void Start()
    {
        PrepareConversation();
    }

    private void InitializeConversationSystem()
    {
        Instance = this;
    }

    public void ImportConversation(ConversationData newConversationData)
    {
        PrepareConversation();

        currentConversationData = newConversationData;

        currentConversationIndex = 0;
        maxConversationIndex = newConversationData.conversationDialogue.Length;

        conversationDialogDisplay.ImportConversationData(currentConversationData);
    }

    private void PrepareConversation()
    {
        if (conversationDialogDisplay == null)
        {
            conversationDialogDisplay = GameObject.FindGameObjectWithTag("ConversationUI").GetComponent<ConversationDialogDisplay>();
        }
    }

    public void StartConversation()
    {
        isConversationActive = true;

        if (conversationCamera != null)
        {
            conversationCamera.ImportConversation(this);
            conversationCamera.ImportCoversationData(currentConversationData);

            conversationCamera.StartConversation();
        }

        conversationDialogDisplay.StartConversation();

        StartConversationDialog();
    }

    public void EndConversation()
    {
        isConversationActive = false;

        if (conversationCamera != null)
        {
            conversationCamera.EndConversation();
        }
        
        conversationDialogDisplay.EndConversation();
    }

    private void StartConversationDialog()
    {
        conversationDialogDisplay.ReadConversationDialog(0);

        if (DoesConversationLineHaveCameraEffect(0))
        {
            conversationCamera.StartCameraEffect(0);
        }
    }

    public void MoveToTargetConversationDialog(int targetConversationIndex)
    {
        if (isConversationActive)
        {
            conversationDialogDisplay.ReadConversationDialog(targetConversationIndex);

            if (DoesConversationLineHaveCameraEffect(targetConversationIndex))
            {
                conversationCamera.StartCameraEffect(targetConversationIndex);
            }
        }
    }

    public void MoveToNextConversationDialog()
    {
        if (isConversationActive)
        {
            if (CanMoveToNextDialog())
            {
                conversationDialogDisplay.ReadConversationDialog(currentConversationIndex);

                if (DoesConversationLineHaveCameraEffect(currentConversationIndex))
                {
                    conversationCamera.StartCameraEffect(currentConversationIndex);
                }
            }
            else
            {
                EndConversation();
            }
        }
    }

    public void ClearDialogBox()
    {
        conversationDialogDisplay.ClearDialogBox();
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

    private bool DoesConversationLineHaveCameraEffect(int conversationIndex)
    {
        if (conversationCamera != null)
        {
            if (currentConversationData.conversationCameraEvents[conversationIndex] != null)
            {
                return true;
            }
        }    

        return false;
    }
}
