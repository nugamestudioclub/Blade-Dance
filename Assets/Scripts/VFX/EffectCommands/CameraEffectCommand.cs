using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectCommand : EffectCommand
{
    private CameraMover cameraMover;

    public string effectType;
    public float duration;
    public float playbackSpeed;

    public CameraEffectCommand(string effectType, float duration, float playbackSpeed)
    {
        this.effectType = effectType;
        this.duration = duration;
        this.playbackSpeed = playbackSpeed;

        cameraMover = Camera.main.GetComponent<CameraMover>();
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void OnBeat()
    {
        throw new System.NotImplementedException();
    }
}
