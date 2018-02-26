using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Scene Event Data", menuName = "Scene/Scene Event Data", order = 1301)]
public class SceneData : ScriptableObject
{
    [Header("Scene Events")]
    public DialogData sceneConversationData;

    [Space(10)]
    public float[] sceneEvenLineTimings;
}
