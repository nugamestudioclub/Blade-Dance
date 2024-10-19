using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BackgroundEffectCommand : EffectCommand
{
    public string filename;
    public float duration;
    public float playbackSpeed;

    public float r;
    public float g;
    public float b;

    private Background background;

    //for video
    public BackgroundEffectCommand(string filename, float duration, float playbackSpeed)
    {
        this.filename = filename;
        this.duration = duration;
        this.playbackSpeed = playbackSpeed;

        background = GameObject.Find("Background").GetComponent<Background>();
    }

    //for color
    public BackgroundEffectCommand(int r, int g, int b)
    {
        this.r = r / 255f;
        this.g = g / 255f;
        this.b = b / 255f;

        background = GameObject.Find("Background").GetComponent<Background>();
    }


    public override void Execute()
    {
        if (filename == null)
        {
            Debug.Log(new Color(r, g, b));
            Debug.Log(background);
            background.ChangeColor(new Color(r, g, b));
        } else
        {
            background.PlayVideo(filename, duration, playbackSpeed);
        }
    }

    public override void OnBeat()
    {
        Debug.Log("called OnBeat");
    }
}
