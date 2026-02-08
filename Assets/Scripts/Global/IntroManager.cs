using System.Collections.Generic;
using UnityEngine;

using Global;

namespace Dlcs
{
    public class IntroManager: MonoBehaviour
    {
        public AudioManager clipPlayer;
        public SubtitlesController subtitles;
        
        public List<GameObject> catSlides;
        public List<GameObject> castSlides;
        public List<GameObject> engSlides;
        
        private List<GameObject> _currentSlides;
        
        private int _currentSlideIndex = 0;
        

        private void OnEnable()
        {
            AudioManager.OnClipFinished += NextSlide;
            GameEvents.LanguageChanged += ChangeLanguage;
        }

        private void OnDisable()
        {
            AudioManager.OnClipFinished -= NextSlide;
            GameEvents.LanguageChanged -= ChangeLanguage;
        }
        
        private void Start()
        {
            _currentSlides = castSlides;
            LoadSlide(_currentSlideIndex);
        }


        private void NextSlide()
        {
            _currentSlides[_currentSlideIndex].SetActive(false);
            _currentSlideIndex++;
            LoadSlide(_currentSlideIndex);
        }
        
        private void LoadSlide(int index)
        {
            _currentSlides[index].SetActive(true);
            clipPlayer.StopCurrentClip();
            //subtitles.CancelTyping();
            //subtitles.PlaySubtitles(_currentSlideIndex);
            
            var clipToPlay = _currentSlides[index].GetComponent<AudioClip>();
            if (clipToPlay)
            {
                clipPlayer.PlayClip(clipToPlay);
            }
            
            
        }

        private void ChangeLanguage(GameManager.GameLanguage language)
        {
            _currentSlides[_currentSlideIndex].SetActive(false);
            
            switch (language)
            {
                case GameManager.GameLanguage.English:
                    _currentSlides = engSlides;
                    break;
                case GameManager.GameLanguage.Spanish:
                    _currentSlides = castSlides;
                    break;
                case GameManager.GameLanguage.Catalan:
                default:
                    _currentSlides = catSlides;
                    break;
            }
            LoadSlide(_currentSlideIndex);
        }
        
        
        
    }
}