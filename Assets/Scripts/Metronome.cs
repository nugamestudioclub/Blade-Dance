using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    float startDspTime;
    int beatsLeadUp;
    float secPerBeat;

    AudioSource[] audioSources;

    int nextSourceToUse = 0;
    int numBeatsSoFar = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = transform.GetComponentsInChildren<AudioSource>();
    }

    public void MakeClicks(float startDspTime, int beatsLeadUp, float secPerBeat)
    {
        this.startDspTime = startDspTime;
        this.beatsLeadUp = beatsLeadUp;
        this.secPerBeat = secPerBeat;
    }

    // Update is called once per frame
    void Update()
    {
        if (numBeatsSoFar < beatsLeadUp)
        {
            double time = AudioSettings.dspTime;

            if (time + 0.25f > (numBeatsSoFar+1) * secPerBeat)
            {
                audioSources[nextSourceToUse].PlayScheduled(startDspTime + (numBeatsSoFar+1) * secPerBeat);
                Debug.Log("Click Scheduled for " + nextSourceToUse + " at " + (startDspTime + (numBeatsSoFar + 1) * secPerBeat));
                nextSourceToUse = (nextSourceToUse + 1) % 8;
                numBeatsSoFar += 1;
            }
        }
        else
        {
            this.enabled = false;
        }
    }
}
