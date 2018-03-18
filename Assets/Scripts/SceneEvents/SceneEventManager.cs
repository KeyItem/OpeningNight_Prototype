using System.Collections;
using UnityEngine;

public class SceneEventManager : MonoBehaviour
{
    private static SceneEventManager _instance;
    public static SceneEventManager Instance { get { return _instance; } }

    [Header("Scene Event Manager Attributes")]
    public SceneData[] sceneData;

    [Space(10)]
    public int currentSceneIndex;

    [Space(10)]
    public bool allFinishedSceneEvents = false;

    public delegate void SceneEventStart();
    public delegate void SceneEventFinished();

    public static event SceneEventStart OnSceneEventStart;
    public static event SceneEventFinished OnSceneEventFinished;

    [Header("Current Scene Event Attributes")]
    public SceneEvent[] sceneEvents;
    
    [Space(10)]
    public SceneEvent currentSceneEvent;

    [Space(10)]
    public StageEvent currentStageEvent;

    [Space(10)]
    public SceneData currentSceneData;

    [Space(10)]
    public DialogData currentDialogData;

    [Space(10)]
    public int currentSceneEventIndex;

    [Space(10)]
    public bool waitingOnStageEvent = false;

    private IEnumerator DialogWaitTime = null;
    private IEnumerator StageEventWaitTime = null;

    [Header("Scene Event Helper Attributes")]
    public int currentSceneEventHelpIndex;

    private int maxSceneEventHelpIndex;

    [Space(10)]
    public float currentSceneEventTime = 0f;
    private float maxSceneEventTime = 0f;

    [Space(10)]
    public bool isSceneEventHelpActive = false;

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

    private void Update()
    {
        ManageSceneEventHelp();
    }

    private void ImportSceneData(SceneData newSceneData)
    {
        currentSceneData = newSceneData;
        currentDialogData = newSceneData.sceneConversationData;
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
        ImportSceneData(sceneData[targetSceneIndex]);

        currentSceneEventIndex = 0;
        currentSceneIndex = targetSceneIndex;

        ConversationSystem.Instance.ImportConversation(currentDialogData);

        StartScene();
    }

    private void PrepareNextScene()
    {
        if (CanMoveToNextScene())
        {
            ImportSceneData(sceneData[currentSceneIndex]);

            currentSceneEventIndex = 0;

            ConversationSystem.Instance.ImportConversation(currentDialogData);

            StartScene();
        }
        else
        {
            CompleteAllScenes();
        }
    }

    private void StartScene()
    {
        currentSceneEvent = sceneEvents[0];

        if (OnSceneEventStart != null)
        {
            OnSceneEventStart();
        }

        AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.MUSIC, currentSceneData.sceneMusic);

        AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.DIALOGUE, currentDialogData.dialogLines[0].targetAudioClip);

        currentSceneEvent.SceneEventStart();

        if (DoesSceneEventContainActorEvent(currentSceneEvent))
        {
            ActorManager.Instance.ReceiveActorActionsData(currentSceneEvent.actorActionsData);
        }

        if (DoesSceneEventContainStageEvent(currentSceneEvent))
        {           
            targetDebugString = "Start Scene :: StageEvent";

            waitingOnStageEvent = true;

            currentStageEvent = currentSceneEvent.targetStageEvent;

            StageEventManager.Instance.PrepareNewStageEvent(currentStageEvent);

            ConversationSystem.Instance.StartConversation();

            if (StageEventWaitTime != null)
            {
                StopCoroutine(StageEventWaitTime);
            }

            StageEventWaitTime = EnableStageEventAfterDelay(currentSceneData.sceneEvenLineTimings[0]);

            StartCoroutine(StageEventWaitTime);
        }
        else
        {
            targetDebugString = "Start Scene :: Normal";

            ConversationSystem.Instance.StartConversation();

            if (DialogWaitTime != null)
            {
                StopCoroutine(DialogWaitTime);
            }

            DialogWaitTime = NextLineAfterDelay(currentSceneData.sceneEvenLineTimings[0]);

            StartCoroutine(DialogWaitTime);
        }
    }

    private void MoveToNextSceneEvent()
    {
        if (OnSceneEventFinished != null)
        {
            OnSceneEventFinished();
        }

        if (CanMoveToNextSceneEvent())
        {
            NextLine(DoesSceneEventContainStageEvent(currentSceneEvent), DoesSceneEventContainActorEvent(currentSceneEvent));       
        }
        else
        {
            PrepareNextScene();
        }
    }

    private void NextLine(bool hasStageEvent, bool hasActorEvent)
    {
        if (OnSceneEventStart != null)
        {
            OnSceneEventStart();
        }

        currentSceneEvent.SceneEventStart();

        AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.DIALOGUE, currentDialogData.dialogLines[currentSceneEventIndex].targetAudioClip);

        if (hasActorEvent)
        {
            ActorManager.Instance.ReceiveActorActionsData(currentSceneEvent.actorActionsData);
        }

        if (hasStageEvent)
        {
            currentStageEvent = currentSceneEvent.targetStageEvent;

            targetDebugString = "Scene :: " + currentSceneEvent.sceneEventName + " :: StageEvent";

            waitingOnStageEvent = true;

            StageEventManager.Instance.PrepareNewStageEvent(currentStageEvent);

            ConversationSystem.Instance.MoveToNextConversationDialog();

            if (StageEventWaitTime != null)
            {
                StopCoroutine(StageEventWaitTime);
            }

            StageEventWaitTime = EnableStageEventAfterDelay(currentSceneData.sceneEvenLineTimings[currentSceneEventIndex]);

            StartCoroutine(StageEventWaitTime);
        }
        else
        {
            targetDebugString = "Scene :: " + currentSceneEvent.sceneEventName + " :: Normal";

            ConversationSystem.Instance.MoveToNextConversationDialog();

            if (DialogWaitTime != null)
            {
                StopCoroutine(DialogWaitTime);
            }

            DialogWaitTime = NextLineAfterDelay(currentSceneData.sceneEvenLineTimings[currentSceneEventIndex]);

            StartCoroutine(DialogWaitTime);
        }
    }

    private void StageEventComplete()
    {
        waitingOnStageEvent = false;

        currentSceneEvent.SceneEventCompleted();

        StageScorer.Instance.StopEventTimer();

        MoveToNextSceneEvent();
    }

    private void ManageSceneEventHelp()
    {
        if (isSceneEventHelpActive)
        {
            currentSceneEventTime -= Time.deltaTime;

            if (currentSceneEventTime <= 0)
            {
                isSceneEventHelpActive = false;

                NextSceneEventHelp();
            }
        }
    }

    private void StartSceneEventHelp()
    {
        currentSceneEventHelpIndex = 0;

        maxSceneEventTime = currentSceneEvent.targetStageEvent.stageEventCompleteTime;
        currentSceneEventTime = maxSceneEventTime;
    }

    private void NextSceneEventHelp()
    {
        if (CanMoveToNextSceneEventHelp())
        {
            maxSceneEventTime = currentSceneEvent.targetStageEvent.stageEventCompleteTime;
            currentSceneEventTime = maxSceneEventTime;

            isSceneEventHelpActive = true;
        }    
    }

    private bool CanMoveToNextSceneEvent()
    {
        currentSceneEventIndex++;

        if (currentSceneEventIndex <= sceneEvents.Length - 1)
        {
            currentSceneEvent = sceneEvents[currentSceneEventIndex];

            return true;
        }

        return false;
    }

    private bool CanMoveToNextScene()
    {
        currentSceneIndex++;

        if (currentSceneIndex <= sceneData.Length - 1)
        {
            return true;
        }

        return false;
    }

    private bool DoesSceneEventContainStageEvent(SceneEvent targetSceneEvent)
    {
        if (targetSceneEvent.targetStageEvent != null)
        {
            return true;
        }

        return false;
    }

    private bool DoesSceneEventContainActorEvent(SceneEvent targetSceneEvent)
    {
        if (targetSceneEvent.actorActionsData.Length > 0)
        {
            return true;
        }

        return false;
    }

    private bool CanMoveToNextSceneEventHelp()
    {
        currentSceneEventHelpIndex++;

        if (currentSceneEventHelpIndex <= maxSceneEventHelpIndex - 1)
        {
            return true;
        }
        else
        {
            currentSceneEventIndex = maxSceneEventHelpIndex;

            return false;
        }
    }

    private void CompleteAllScenes()
    {
        StageEventManager.OnStageEventCompleted -= StageEventComplete;

        CurtainManager.Instance.MoveCurtain();

        currentSceneIndex = 0;
        currentSceneEventIndex = 0;

        currentStageEvent = null;
        currentSceneData = null;
        currentDialogData = null;

        targetDebugString = "All Scenes Complete";

        allFinishedSceneEvents = true;
    }

    private IEnumerator NextLineAfterDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            float startTime = Time.time;

            while (Time.time < startTime + delayTime)
            {
                yield return null;
            }
        }

        currentSceneEvent.SceneEventCompleted();

        ConversationSystem.Instance.ClearDialogBox();

        MoveToNextSceneEvent();

        yield return null;
    }

    private IEnumerator EnableStageEventAfterDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            float startTime = Time.time;

            while (Time.time < startTime + delayTime)
            {
                yield return null;
            }
        }

        ConversationSystem.Instance.ClearDialogBox();

        StageEventManager.Instance.StartStageEvent(currentStageEvent);

        StageScorer.Instance.StartEventTimer();

        yield return null;
    }

    private IEnumerator StartPlayAfterDelay(float delayTime)
    {
        if (delayTime > 0)
        {
            float startTime = Time.time;

            while (Time.time < startTime + delayTime)
            {
                yield return null;
            }
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
