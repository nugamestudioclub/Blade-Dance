using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectCommand
{
    public abstract void Execute();
}


public class BackgroundEffectCommand : EffectCommand
{
    public string filename;
    public float duration;
    public float playbackSpeed;

    private Background background;

    public BackgroundEffectCommand(string filename, float duration, float playbackSpeed)
    {
        this.filename = filename;
        this.duration = duration;
        this.playbackSpeed = playbackSpeed;

        background = GameObject.Find("Background").GetComponent<Background>();
    }

    public override void Execute()
    {
        background.PlayVideo(filename, duration, playbackSpeed);
    }
}

