using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Background : MonoBehaviour
{

    private string filepath = "Assets/Videos/";

    [SerializeField]
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // plays video at the specified framerate. If another video is playing before, it will stop that video and play this video
    // optional boolean to sync it to frames per beat (byBeat = true) or frames per second (byBeat = false)
    public void PlayVideo(string filename, float playbackSpeed = 1)
    {
        videoPlayer.targetCameraAlpha = 0.5F;

        videoPlayer.playbackSpeed = playbackSpeed;

        videoPlayer.url = filepath + filename + ".mp4";

        // Skip the first 100 frames.
        videoPlayer.frame = 100;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    public void ResumeVideo()
    {
        videoPlayer.Play();
    }

    
}
