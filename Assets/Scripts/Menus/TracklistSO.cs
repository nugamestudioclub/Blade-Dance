using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tracklist", menuName = "ScriptableObjects/New Tracklist")]
public class TracklistSO : ScriptableObject
{
    public List<SongSO> songs;
}
