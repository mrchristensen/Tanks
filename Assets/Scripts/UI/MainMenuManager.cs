using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static GameManager;

public class MainMenuManager : MonoBehaviour
{

    public string gameScene;
    
    public void Play()
    {
        Debug.Log("Play");
        SceneManager.LoadScene(gameScene);
    }

    public void Quit()
    {
        Debug.Log ("Quit");
        Application.Quit();
    }
}
