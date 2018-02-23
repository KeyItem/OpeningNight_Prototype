using System.Collections;
using UnityEngine;

public class SceneEventManager : MonoBehaviour
{
    private static SceneEventManager _instance;
    public static SceneEventManager Instance { get { return _instance; } }

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

    [Header("DEBUG")]
    public float playStartDelay = 1f;

    [Space(10)]
    public bool canShowDebug = false;

    private string targetDebugString;

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
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void ImportSceneEventData(SceneEventData newSceneEventData)
    {
        currentSceneEventData = newSceneEventData;
        currentConversationData = newSceneEventData.sceneConversationData;
    }

    public void PrepareStartingScene()
    {
        StageEventManager.OnStageEventCompleted += StageEventComplete;

        PropManager.Instance.StartPropSetup();

        CurtainManager.Instance.MoveCurtain();

        CrowdManager.Instance.StartCrowd();

        StartCoroutine(StartPlayAfterDelay(playStartDelay));
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
            targetDebugString = "Start Conversation :: StageEvent";

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
            targetDebugString = "Start Conversation :: Normal";

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
        if (hasStageEvent)
        {
            targetDebugString = "Line :: " + currentLineIndex + " :: StageEvent";

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
            targetDebugString = "Line :: " + currentLineIndex;

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

        StageScorer.Instance.StopEventTimer();

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

        CurtainManager.Instance.MoveCurtain();

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

        StageScorer.Instance.StartEventTimer();

        yield return null;
    }

    private IEnumerator StartPlayAfterDelay(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        PrepareScene(0);

        yield return null;
    }

    private void OnGUI()
    {
        if (canShowDebug)
        {
            GUI.Box(new Rect(Screen.width - 350, 0, 300, 25), "");
            GUI.Label(new Rect(Screen.width - 350, 0, 300, 25), targetDebugString);
        }
    }
}
