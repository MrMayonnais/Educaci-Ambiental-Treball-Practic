using System;
using Global;
using Intro;
using UnityEngine;

namespace Dlcs
{
    public class DialoguePlayer : MonoBehaviour
    {
    
        private AudioSource _currentSource;

        private void OnEnable()
        {
            ISlidesController.OnNextSlide += StopCurrentClip;
            ISlidesController.OnSlideShowFinished += StopCurrentClip;
            GameEvents.LanguageChanged += StopForLanguageChange;
        }
        
        private void OnDisable()
        {
            ISlidesController.OnNextSlide -= StopCurrentClip;
            ISlidesController.OnSlideShowFinished -= StopCurrentClip;
            GameEvents.LanguageChanged -= StopForLanguageChange;
        }

        public void PlayClip(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioClip is null");
                return;
            }

            StopCurrentClip();

            _currentSource = gameObject.AddComponent<AudioSource>();
            _currentSource.clip = clip;
            _currentSource.Play();
        }

        private void StopForLanguageChange(GameManager.GameLanguage language)
        {
            StopCurrentClip();
        }

        public void StopCurrentClip()
        {
            if (_currentSource != null)
            {
                _currentSource.Stop();
                Destroy(_currentSource);
                _currentSource = null;
            }
        }


    }
}