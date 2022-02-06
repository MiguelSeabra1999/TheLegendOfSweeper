using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> _music = new List<AudioSource>();
    [SerializeField] private List<AudioSource> _sfx = new List<AudioSource>();
    [SerializeField] private float _changeDuration;
    private int _currentMusic = 0;
    private float _startTIme;
    void Awake()
    {
        _startTIme = Time.time;
    }
    public void PlayMusic(int musicIndex)
    {
        if(musicIndex != _currentMusic)
        {
            StartCoroutine(ChangeTrack(musicIndex));
        }
    }
    private IEnumerator ChangeTrack(int newTrack)
    {
        float startTime = Time.time;
        float percent = 0;
        float startVolume = _music[_currentMusic].volume;
  
        while(percent < 1)
        {
            percent = (Time.time - startTime)/_changeDuration;
            _music[_currentMusic].volume = Mathf.Lerp(startVolume,0,percent);
            _music[newTrack].volume = Mathf.Lerp(0,1,percent);
            yield return null;
        }

        _currentMusic = newTrack;
    }

    public void PlaySFX(int index)
    {
        if(Time.time - _startTIme > 3)
            _sfx[index].Play();
    }
}
