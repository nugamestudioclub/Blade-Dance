using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObjects/New Level")]
public class LevelSO : ScriptableObject
{
    public string contentFilePath;
    public int difficulty;
}
