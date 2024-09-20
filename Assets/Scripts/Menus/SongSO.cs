using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "ScriptableObjects/New Song")]
public class SongSO : ScriptableObject
{
    public Sprite songIcon;
    public string songName;
    public string bandName;
    public List<LevelSO> levels;

    // we assume that levels contains its LevelSOs sorted in ascending
    // order of difficulty and has no LevelSOs of duplicate difficulty
    // we also assume every SongSO has at least one LevelSO

    public string GetDifficultyList()
    {
        string result = "";
        foreach (LevelSO level in levels)
        {
            result += level.difficulty + ", ";
        }
        return result.Substring(0, 3 * levels.Count - 2);
    }
}
