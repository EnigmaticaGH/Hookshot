﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public Canvas pauseScreen;

    private float oldTime;

    void Start()
    {
        pauseScreen.enabled = false;
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            pause();
            
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void pause()
    {
        pauseScreen.enabled = !pauseScreen.enabled;
            if (pauseScreen.enabled)
            {
                oldTime = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = oldTime;
            }
    }
}
