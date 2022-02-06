using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class UImanager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private GameObject _pause;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _gameOverDurationText = 0.5f;
    [SerializeField] private float _gameOverDurationBar = 0.5f;
    [SerializeField] private float _revealDuration = 1f;
    [SerializeField] private float _downBarHeight = 0.5f;
    [SerializeField] private float _upBarHeight = 0.5f;
    private bool _paused;

    void Start()
    {
        Invoke("Reveal",2f);
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _paused = !_paused;
            _pause.SetActive(_paused);
            if(_paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
    public void Reveal()
    {
        StartCoroutine(RevealGame());
    }
    [ContextMenu("GameOver")]
    public void SetGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    [ContextMenu("Win")]
    public void SetWin()
    {
        PlayerPrefs.SetInt("hasWon",1);
        StartCoroutine(WinRoutine());
    }
    private IEnumerator GameOverRoutine()
    {
        float percent = 0;
        float startTime = Time.time;
        
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_gameOverDurationBar;
            _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x,Mathf.Lerp(_upBarHeight,_downBarHeight,percent));
            yield return null;
        }

        percent = 0;
        startTime = Time.time;
        _canvasGroup.interactable = true;
_canvasGroup.blocksRaycasts = true;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_gameOverDurationText;
            _canvasGroup.alpha = Mathf.Lerp(0,1,percent);
            PressAnythingToRestart();
            yield return null;
        }
        while(true)
        {
            PressAnythingToRestart();
            yield return null;
        }
    }
    private IEnumerator WinRoutine()
    {
        float percent = 0;
        float startTime = Time.time;
        _text.text = "You Win!";
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_gameOverDurationBar;
            _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x,Mathf.Lerp(_upBarHeight,_downBarHeight,percent));
            yield return null;
        }

       // _canvasGroup.gameObject.SetActive(true);
_canvasGroup.interactable = true;
_canvasGroup.blocksRaycasts = true;
        percent = 0;
        startTime = Time.time;
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_gameOverDurationText;
            _canvasGroup.alpha = Mathf.Lerp(0,1,percent);
            PressAnythingToGoToMenu();
            yield return null;
        }
        while(true)
        {
            PressAnythingToGoToMenu();
            yield return null;
        }
    }

    private void PressAnythingToRestart()
    {
       /* if(Input.anyKeyDown)
            Restart();*/
    }
    private void PressAnythingToGoToMenu()
    {
        /*if(Input.anyKeyDown)
            SceneManager.LoadScene( "Menu" ) ;*/
    }
    public void Restart()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ) ;
    }
    public void ToMenu()
    {
        SceneManager.LoadScene( "Menu" ) ;
    }
    private IEnumerator RevealGame()
    {
        float percent = 0;
        float startTime = Time.time;
        
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_revealDuration;
            _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x,Mathf.Lerp(_downBarHeight,_upBarHeight,percent));
            yield return null;
        }
    }
}
