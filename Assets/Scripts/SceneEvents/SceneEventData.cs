using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Scene Event Data", menuName = "Scene/Scene Event Data", order = 1301)]
public class SceneEventData : ScriptableObject
{
    [Header("Scene Events")]
    public ConversationData sceneConversationData;

    [Space(10)]
    public bool[] sceneEventHasStageEvent;

    [Space(10)]
    public float[] sceneEvenLineTimings;
}
