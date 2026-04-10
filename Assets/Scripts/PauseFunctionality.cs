using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseFunctionality : MonoBehaviour
{
    private static bool isPaused = false;

    public GameObject pauseMenu;

    // Returns true if the game is paused.
    public static bool getIsPaused()
    {
        return isPaused;
    }

    void TogglePause()
    {
        if (isPaused)
        {
            UnPause();
        } else
        {
            Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (KeybindManager.PressedPause())
        {
            TogglePause();
        }
    }

    void OnDestroy()
    {
        UnPause();
    }
}
