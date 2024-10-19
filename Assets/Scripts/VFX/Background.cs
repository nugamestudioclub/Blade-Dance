using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Background : MonoBehaviour
{

    private string filepath = "Assets/Videos/";

    [SerializeField]
    private GameObject video;
    private VideoPlayer videoPlayer;
    private SpriteRenderer videoSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = video.GetComponent<VideoPlayer>();
        videoSpriteRenderer = video.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // plays video at the specified framerate. If another video is playing before, it will stop that video and play this video
    // optional boolean to sync it to frames per beat (byBeat = true) or frames per second (byBeat = false)
    public void PlayVideo(string filename, float duration, float playbackSpeed = 1)
    {
        videoPlayer.enabled = true;
        videoPlayer.targetCameraAlpha = 0.5F;

        videoPlayer.playbackSpeed = playbackSpeed;

        videoPlayer.url = filepath + filename + ".mp4";

        // Skip the first 100 frames.
        videoPlayer.frame = 100;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
        StartCoroutine(Timer(duration));
    }

    public void ChangeColor(Color color)
    {
        videoPlayer.enabled = false;
        videoSpriteRenderer.color = color;
    }

    IEnumerator Timer(float duration)
    {
        yield return new WaitForSeconds(duration);
        StopVideo();
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
