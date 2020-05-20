using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameMode { DEATH_MATCH = 0, CAPTURE_THE_FLAG = 1, KING_OF_THE_HILL = 2}
    
    public GameMode gameMode = GameMode.DEATH_MATCH;
    public int numPlayers;
    public bool teams;
    public int numRoundsToWin;
    
    static GameManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetGameMode(Dropdown gameModeDropdown)
    {
        Debug.Log("Gamemode dropdown updated to: " + gameModeDropdown.value);
        Debug.Log("Gamemode updated to: " + (GameMode)(gameModeDropdown.value));
        gameMode = (GameMode)(gameModeDropdown.value);
    }

    public void SetTeams(Toggle teamsToggle)
    {
        Debug.Log("Teams: " + teamsToggle.isOn);
        teams = teamsToggle.isOn;
    }
}
