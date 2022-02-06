using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject HardModeButton;
    void Awake()
    {
        if(PlayerPrefs.HasKey("hasWon"))
        {
            if(PlayerPrefs.GetInt("hasWon") == 1)
                HardModeButton.SetActive(true);
        }
    }
    public void Continue()
    {
        SceneManager.LoadScene( "Main 1" ) ;
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("hasBoots",0);
        PlayerPrefs.SetInt("hasShield",0);
        SceneManager.LoadScene( "Main 1" ) ;
    }
    public void StartPractice()
    {
        SceneManager.LoadScene( "Main" ) ;
    }
    public void HardMode()
    {
        SceneManager.LoadScene( "Hard" ) ;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
