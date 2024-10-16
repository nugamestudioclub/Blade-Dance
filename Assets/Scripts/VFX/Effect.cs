using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public abstract void Play();
}


public class BackgroundEffect : Effect
{
    public override void Play()
    {
        //default behavior for bgeffect
    }
}
