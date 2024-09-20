using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMarkerPlacer : MonoBehaviour
{
    // Assumes we have 12 children which are all line segments of length 1
    public void PlaceMarkers(float distance)
    {
        float petalOffset = 1f;

        for (int i = 0; i < 3; i++)
        {
            Transform target = transform.GetChild(i);
            float distanceFromCenter = petalOffset + distance * (1 + i);
            target.position = new Vector3(0f, distanceFromCenter, 0f);
            //target.localScale = new Vector3(2f * distanceFromCenter, 1f, 1f);
        }

        for (int i = 0; i < 3; i++)
        {
            Transform target = transform.GetChild(i + 3);
            float distanceFromCenter = petalOffset + distance * (1 + i);
            target.position = new Vector3(0f, -distanceFromCenter, 0f);
            //target.localScale = new Vector3(2f * distanceFromCenter, 1f, 1f);
        }

        for (int i = 0; i < 3; i++)
        {
            Transform target = transform.GetChild(i + 6);
            float distanceFromCenter = petalOffset + distance * (1 + i);
            target.position = new Vector3(distanceFromCenter, 0f, 0f);
            //target.localScale = new Vector3(2f * distanceFromCenter, 1f, 1f);
        }

        for (int i = 0; i < 3; i++)
        {
            Transform target = transform.GetChild(i + 9);
            float distanceFromCenter = petalOffset + distance * (1 + i);
            target.position = new Vector3(-distanceFromCenter, 0f, 0f);
            //target.localScale = new Vector3(2f * distanceFromCenter, 1f, 1f);
        }
    }
}
