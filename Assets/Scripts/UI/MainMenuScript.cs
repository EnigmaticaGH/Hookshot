using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {


    public void loadLevel(string level)
    {
        Application.LoadLevel(level);
    }

    public void closeGame()
    {
        Application.Quit();
    }

}
