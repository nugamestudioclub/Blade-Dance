using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LevelBase
{
    void Begin(LevelRunner runner, float distanceToCenter, float noteSpeed, float secPerBeat);

    void AtBeat(float beat);

    bool IsLevelComplete();
}
