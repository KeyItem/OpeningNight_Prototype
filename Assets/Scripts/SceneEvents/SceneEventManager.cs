using System.Collections;
using UnityEngine;

public class SceneEventManager : MonoBehaviour
{
    public static SceneEventManager Instance;

    [Header("Scene Event Manager Attributes")]
    public SceneEventData[] sceneEvents;

    [Space(10)]
    public int currentSceneIndex;

    [Space(10)]
    public bool allFinishedSceneEvents = false;

    [Header("Current Scene Event Attributes")]
    public SceneEventData currentSceneEventData;

    [Space(10)]
    public ConversationData currentConversationData;

    [Space(10)]
    public int currentLineIndex;

    [Space(10)]
    public bool waitingOnStageEvent = false;

    private IEnumerator DialogWaitTime = null;
    private IEnumerator StageEventWaitTime = null;

    private void Awake()
    {
        InitializeSceneEventManager();
    }

    private void Start()
    {
        PrepareStartingScene();
    }

    private void InitializeSceneEventManager()
    {
        Instance = this;
    }

    private void ImportSceneEventData(SceneEventData newSceneEventData)
    {
        currentSceneEventData = newSceneEventData;
        currentConversationData = newSceneEventData.sceneConversationData;
    }

    public void PrepareStartingScene()
    {
        StageEventManager.OnStageEventCompleted += StageEventComplete;

        PrepareScene(0);
    }

    public void PrepareScene(int targetSceneIndex)
    {
        ImportSceneEventData(sceneEvents[targetSceneIndex]);

        currentLineIndex = 0;
        currentSceneIndex = targetSceneIndex;

        ConversationSystem.Instance.ImportConversation(currentConversationData);

        StartScene();
    }

    private void PrepareNextScene()
    {
        if (CanMoveToNextScene())
        {
            ImportSceneEventData(sceneEvents[currentSceneIndex]);

            currentLineIndex = 0;

            ConversationSystem.Instance.ImportConversation(currentConversationData);

            StartScene();
        }
        else
        {
            CompleteAllScenes();
        }
    }

    private void StartScene()
    {
        if (DoesSceneEventContainStageEvent(0))
        {
            Debug.Log("Start Conversation :: StageEvent");
            Debug.Log("Playing Line " + 0);

            waitingOnStageEvent = true;

            ConversationSystem.Instance.StartConversation();

            if (StageEventWaitTime != null)
            {
                StopCoroutine(StageEventWaitTime);
            }

            StageEventWaitTime = EnableStageEventAfterDelay(currentSceneEventData.sceneEvenLineTimings[0]);

            StartCoroutine(StageEventWaitTime);
        }
        else
        {
            Debug.Log("Start Conversation :: Normal");
            Debug.Log("Playing Line " + 0);

            ConversationSystem.Instance.StartConversation();

            if (DialogWaitTime != null)
            {
                StopCoroutine(DialogWaitTime);
            }

            DialogWaitTime = NextLineAfterDelay(currentSceneEventData.sceneEvenLineTimings[0]);

            StartCoroutine(DialogWaitTime);
        }
    }

    private void MoveToNextLineOfDialog()
    {
        if (CanMoveToNextLineOfDialogue())
        {
            if (DoesSceneEventContainStageEvent(currentLineIndex))
            {
                NextLine(true);
            }
            else
            {
                NextLine(false);
            }
        }
        else
        {
            PrepareNextScene();
        }
    }

    private void NextLine(bool hasStageEvent)
    {
        Debug.Log("Playing Line " + currentLineIndex);

        if (hasStageEvent)
        {
            waitingOnStageEvent = true;

            ConversationSystem.Instance.MoveToNextConversationDialog();

            if (StageEventWaitTime != null)
            {
                StopCoroutine(StageEventWaitTime);
            }

            StageEventWaitTime = EnableStageEventAfterDelay(currentSceneEventData.sceneEvenLineTimings[currentLineIndex]);

            StartCoroutine(StageEventWaitTime);
        }
        else
        {
            ConversationSystem.Instance.MoveToNextConversationDialog();

            if (DialogWaitTime != null)
            {
                StopCoroutine(DialogWaitTime);
            }

            DialogWaitTime = NextLineAfterDelay(currentSceneEventData.sceneEvenLineTimings[currentLineIndex]);

            StartCoroutine(DialogWaitTime);
        }
    }

    private void StageEventComplete()
    {
        waitingOnStageEvent = false;

        MoveToNextLineOfDialog();
    }

    private bool CanMoveToNextLineOfDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex <= currentConversationData.conversationDialogue.Length - 1)
        {
            return true;
        }

        return false;
    }

    private bool CanMoveToNextScene()
    {
        currentSceneIndex++;

        if (currentSceneIndex <= sceneEvents.Length - 1)
        {
            return true;
        }

        return false;
    }

    private bool DoesSceneEventContainStageEvent(int currentLineIndex)
    {
        if (currentSceneEventData.sceneEventHasStageEvent[currentLineIndex])
        {
            return true;
        }

        return false;
    }

    private void CompleteAllScenes()
    {
        StageEventManager.OnStageEventCompleted += StageEventComplete;

        currentSceneIndex = 0;
        currentLineIndex = 0;

        currentSceneEventData = null;
        currentConversationData = null;

        Debug.Log("All Scenes Complete");

        allFinishedSceneEvents = true;
    }

    private IEnumerator NextLineAfterDelay(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        ConversationSystem.Instance.ClearDialogBox();

        MoveToNextLineOfDialog();

        yield return null;
    }

    private IEnumerator EnableStageEventAfterDelay(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        ConversationSystem.Instance.ClearDialogBox();

        StageEventManager.Instance.RequestStageEvent();

        yield return null;
    }

    /*
     *  private void StartSceneList()
    {
        StageEventManager.OnStageEventCompleted += StageEventFinished;

        currentSceneIndex = 0;

        ImportSceneEventData(sceneEvents[0]);

        Debug.Log("Setting Up Scene");
    }

    public void ImportSceneEventData(SceneEventData newSceneEventData)
    {
        currentSceneEventData = newSceneEventData;
        currentConversationData = newSceneEventData.sceneConversationData;
        currentLineIndex = 0;

        ConversationSystem.Instance.ImportConversation(currentConversationData);

        Debug.Log("Importing Scene Event Data");
    }

    public void StartCurrentSceneEvent()
    {
        ConversationSystem.Instance.StartConversation();

        currentLineIndex = -1;

        NextLine();

        Debug.Log("Starting Scene");
    }

    private void NextLine()
    {
        if (CanMoveToNextLine())
        {
            RelayConversationLine();

            Debug.Log("WTF");

            if (DoesLineContainStageEvent(currentLineIndex))
            {
                if (StageEventDelay != null)
                {
                    StopCoroutine(StageEventDelay);
                }

                StageEventDelay = RequestStageEventAfterDelay(currentSceneEventData.sceneEvenLineTimings[currentLineIndex]);

                StartCoroutine(StageEventDelay);
            }
            else
            {
                if (NextLineEventDelay != null)
                {
                    StopCoroutine(NextLineEventDelay);
                }

                NextLineEventDelay = RequestNextLineAfterDelay(currentSceneEventData.sceneEvenLineTimings[currentLineIndex]);

                StartCoroutine(NextLineEventDelay);
            }
        }
        else
        {
            CompleteCurrentScene();
        }
    }

    public void CompleteCurrentScene()
    {
        currentLineIndex = 0;

        currentSceneEventData = null;
        currentConversationData = null;

        Debug.Log("Complete Scene");
    }

    private void CompleteAllScenes()
    {
        currentSceneIndex = 0;
        currentLineIndex = 0;

        currentSceneEventData = null;
        currentConversationData = null;

        StageEventManager.OnStageEventCompleted -= NextLine;

        Debug.Log("Complete Scene List");
    }

    private void MoveToNextSceneEvent()
    {
        if (CanMoveToNextScene())
        {
            ImportSceneEventData(sceneEvents[currentSceneIndex]);
        }
        else
        {
            CompleteAllScenes();
        }
    }

    private void RelayConversationLine()
    {
        Debug.Log("Send Line to Conversation System");

        ConversationSystem.Instance.MoveToNextConversationDialog();
    }

    private void StageEventFinished()
    {
        NextLine();
    }

    private bool CanMoveToNextScene()
    {
        if (sceneEvents.Length > 0)
        {
            currentSceneIndex++;

            if (currentSceneIndex <= sceneEvents.Length - 1)
            {
                return true;
            }
        }

        return false;
    }

    private bool CanMoveToNextLine()
    {
        Debug.Log("True");

        currentLineIndex++;

        if (currentLineIndex <= currentSceneEventData.sceneConversationData.conversationDialogue.Length)
        {
            return true;
        }

        Debug.Log("False");

        return false;
    }

    private bool DoesLineContainStageEvent(int sceneEventIndex)
    {
        if (currentSceneEventData.sceneEventHasStageEvent[sceneEventIndex])
        {
            return true;
        }

        return false;
    }

    private IEnumerator RequestStageEventAfterDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
        }

        StageEventManager.Instance.RequestStageEvent();

        StageEventDelay = null;

        yield return null;
    }

    private IEnumerator RequestNextLineAfterDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
        }

        NextLineEventDelay = null;

        NextLine();

        yield return null;
    }

    private IEnumerator RequestStartDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
        }

        NextLine();

        yield return null;

    }
     */
}
