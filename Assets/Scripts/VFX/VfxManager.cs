using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManager : MonoBehaviour
{

    private Background bg;

    private float start;
    private float thing;

    // Start is called before the first frame update
    void Start()
    {
        bg = GetComponentInChildren<Background>();
        bg.PlayVideo("bad-apple", 0.3f);

        start = Time.time;
        thing = start;
    }

    // Update is called once per frame
    void Update()
    {
        thing += Time.deltaTime;
        if (thing - start > 4)
        {
            bg.StopVideo();
        }
    }

    
}
