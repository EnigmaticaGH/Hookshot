using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject ControlUI;

    private float oldTime;

    void Start()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
        }
            
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void pause()
    {
        pauseScreen.SetActive(!pauseScreen.activeInHierarchy);
        ControlUI.SetActive(!ControlUI.activeInHierarchy);
            if (pauseScreen.activeInHierarchy)
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
