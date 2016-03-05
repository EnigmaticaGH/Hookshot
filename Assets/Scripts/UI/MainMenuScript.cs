using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuScript : MonoBehaviour {


    public void loadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void closeGame()
    {
        Application.Quit();
    }

}
