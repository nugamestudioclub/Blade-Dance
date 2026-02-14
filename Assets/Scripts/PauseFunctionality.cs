using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseFunctionality : MonoBehaviour
{
    private static KeyCode PAUSE_KEY = KeyCode.P;
    private bool isPaused = false;

    void TogglePause()
    {
        Debug.Log("Is Pausing!");

        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
            
        }

        else
        {
            isPaused = true;
            Time.timeScale = 0;
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(PAUSE_KEY))
        {
            TogglePause();
        }
    }
}
