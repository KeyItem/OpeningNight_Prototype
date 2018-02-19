using UnityEngine;
using System;
using System.Collections.Generic;

public class BeatDetector : MonoBehaviour
{
    [Header("Beat Detection Attributes")]
    public AudioProcessor audioProcessor;

    private AudioSource audioSource;

    [Space(10)]
    public List<float> beatList;

    private float lastBeatTime = 0f;

	void Start ()
	{
        IntializeDetector();
	}

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.Space))
       {
           StartClip();
       }

       if (Input.GetKeyDown(KeyCode.E))
       {
           ShowBeatsPerSecond();
       }

       if (Input.GetKeyDown(KeyCode.R))
       {
           ShowAverageBeatTime();
       }

       if (Input.GetKeyDown(KeyCode.T))
        {
            ShowBeatsPerMinute();
        }
    }

    private void IntializeDetector()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioProcessor == null)
        {
            audioProcessor = FindObjectOfType<AudioProcessor>();
        }

        audioProcessor.onBeat.AddListener(onOnbeatDetected);
    }

    void onOnbeatDetected ()
	{
        RecordBeat();
	}

    private void StartClip()
    {
        lastBeatTime = Time.time;

        beatList.Clear();

        audioSource.Play();
    }

    private void RecordBeat()
    {
        float currentBeatTime = 0;

        currentBeatTime = Time.time - lastBeatTime;

        beatList.Add(currentBeatTime);

        lastBeatTime = Time.time;
    }

    private void ShowAverageBeatTime()
    {
        float meanSecondsPerBeat = ReturnAverageBeatTime();

        Debug.Log("Average Beat Time :: " + meanSecondsPerBeat);
    }

    private void ShowBeatsPerMinute()
    {
        int beatsPerMinute = ReturnBeatsPerMinute();

        Debug.Log("Beats Per Minute :: " + beatsPerMinute);
    }

    private void ShowBeatsPerSecond()
    {
        float beatsPerSecond = ReturnBeatsPerSecond();

        Debug.Log("Beats Per Second :: " + beatsPerSecond);
    }

    private float ReturnBeatsPerSecond()
    {
        float timeElapsed = lastBeatTime;

        float beatCount = beatList.Count;

        return timeElapsed / beatCount;
    }

    private int ReturnBeatsPerMinute()
    {
        float newBeatsPerMinute = 0;

        float averageBeatTime = ReturnAverageBeatTime();

        newBeatsPerMinute = 60f / averageBeatTime;

        int newBPM = (int)newBeatsPerMinute;

        return newBPM;
    }

    private float ReturnAverageBeatTime()
    {
        float newBeatAverageTime = 0f;
        float totalBeatTime = 0;

        for (int i = 1; i < beatList.Count; ++i)
        {
            totalBeatTime += beatList[i];
        }

        newBeatAverageTime = totalBeatTime / beatList.Count;

        return newBeatAverageTime;
    }
}
