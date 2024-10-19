using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectCommand
{
    public abstract void Execute();
    public abstract void OnBeat();
}

