using System;
using Global;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip introMusic;
    public AudioClip gameMusic;

    public float audioTransitionDuration = 0.5f;
    
    private AudioSource  _audioSource;
    
    private Tween _volumeTween;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (_audioSource == null)  _audioSource = this.AddComponent<AudioSource>();
        
        _audioSource.clip = introMusic;
        _audioSource.loop = true;
        _audioSource.Play();
    }
    

    private void OnEnable()
    {
        GameEvents.PlayGameMusic += PlayGameMusic;
        GameEvents.PlayIntroMusic += PlayIntroMusic;
    }
    
    private void OnDisable()
    {
        GameEvents.PlayIntroMusic -= PlayIntroMusic;
        GameEvents.PlayGameMusic -= PlayGameMusic;

    }

    private void PlayGameMusic()
    {
        if (_audioSource.isPlaying && _audioSource.clip != introMusic)
        {
            this.AddComponent<AudioSource>();
        
            var volume = _audioSource.volume;
            if(_volumeTween.isAlive) _volumeTween.Stop();
            _volumeTween = Tween.AudioVolume(_audioSource, volume, 0f, audioTransitionDuration/2, Easing.Standard(Ease.Linear))
                .OnComplete(() =>
                {
                    _audioSource.clip = gameMusic;
                    _volumeTween = Tween.AudioVolume(_audioSource, 0f, volume, audioTransitionDuration/2, Easing.Standard(Ease.Linear));
                });
        }
    }

    private void PlayIntroMusic()
    {
        if (_audioSource.isPlaying && _audioSource.clip != gameMusic)
        {
            this.AddComponent<AudioSource>();
        
            var volume = _audioSource.volume;
            if(_volumeTween.isAlive) _volumeTween.Stop();
            _volumeTween = Tween.AudioVolume(_audioSource, volume, 0f, audioTransitionDuration/2, Easing.Standard(Ease.Linear))
                .OnComplete(() =>
                {
                    _audioSource.clip = introMusic;
                    _volumeTween = Tween.AudioVolume(_audioSource, 0f, volume, audioTransitionDuration/2, Easing.Standard(Ease.Linear));
                });
        }
    }
}
