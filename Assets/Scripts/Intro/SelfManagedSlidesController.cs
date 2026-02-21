using System.Collections.Generic;
using Dlcs;
using Global;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Intro
{
    public class SelfManagedSlidesController : MonoBehaviour, ISlidesController
    {   
        [Header("UI References")]
        public List<GameObject> catSlides;
        public List<GameObject> casSlides;
        public List<GameObject> engSlides;
        
        public SubtitlesController subtitles;
        public DialoguePlayer dialoguePlayer;
    
        public Image fadeImage;
        
        [Header("Animation Settings")]
        public Transform entrancePosition;
        public Transform normalPosition;
        public Transform exitPosition;
        
        public float fadeDuration = 1f;
        public float slideTransitionDuration = 1f;
        
        
        private Button _continueButton;
        
        private int _currentSlideIndex = 0;
        
        private List<GameObject> _currentSlides;
        private GameObject _currentSlide;
        private GameObject _previousSlide;
        
        private Tween _fadeTween;
        private Tween _enterSlideTween;
        private Tween _exitSlideTween;
        
        private GameManager.GameLanguage _currentLanguage = GameManager.GameLanguage.Catalan;


        private void Awake()
        {
            _continueButton = subtitles.gameObject.GetChildByNameRecursively("ContinueButton").GetComponent<Button>();
            _continueButton.onClick.AddListener(NextSlide);
        }
        
        private void Start()
        {
            _currentLanguage = LanguageChanger.CurrentLanguage();
            
            GetCurrentSlides();
            AnimateEntrance();
            LoadSlide(_currentSlide);
            ISlidesController.OnStartSlideShow?.Invoke();
        }

        private void OnEnable()
        {
            GameEvents.LanguageChanged += ChangeLanguage;
        }
        
        private void OnDisable()
        {
            GameEvents.LanguageChanged -= ChangeLanguage;
        }
        
        public void NextSlide()
        {
            _currentSlideIndex++;
            if (_currentSlideIndex >= _currentSlides.Count) FinishSlideShow();
            else
            {
                _previousSlide = _currentSlide;
                _currentSlide = _currentSlides[_currentSlideIndex];
                LoadSlide(_currentSlide);
                ISlidesController.OnNextSlide?.Invoke();
            }
        }

        public void LoadSlide(GameObject slide)
        {
            _currentSlide.SetActive(true);
            SwapSlide();
            subtitles.CancelTyping();
        }

        public void SwapSlide()
        {
            AnimateSlideEntrance(_currentSlide);
            AnimateSlideExit(_previousSlide);
        }
        
        public void FinishSlideShow()
        {   
            ISlidesController.OnSlideShowFinished?.Invoke();
            AnimateExit();
        }

        public void ChangeLanguage(GameManager.GameLanguage newLanguage)
        {
            _previousSlide = _currentSlide;
            _currentLanguage = newLanguage;
            GetCurrentSlides();
            LoadSlide(_currentSlide);
        }

        public void AnimateSlideEntrance(GameObject slide)
        {
            if(_enterSlideTween.isAlive) _enterSlideTween.Stop();
            _continueButton.interactable = false;
            _enterSlideTween = Tween.Position(slide.transform, 
                entrancePosition.position, 
                normalPosition.position, 
                slideTransitionDuration, 
                Easing.Standard(Ease.InOutCubic)).OnComplete(()=>
                {
                    dialoguePlayer.PlayClip(slide.GetComponent<AudioSource>().clip);
                    subtitles.PlaySubtitles(_currentSlideIndex);
                    _continueButton.interactable = true;
                });
        }

        public void AnimateSlideExit(GameObject slide)
        {
            if (slide)
            {
                if(_exitSlideTween.isAlive) _exitSlideTween.Stop();
                _exitSlideTween = Tween.Position(slide.transform, 
                    normalPosition.position, 
                    exitPosition.position, 
                    slideTransitionDuration, 
                    Easing.Standard(Ease.InOutCubic)).OnComplete(()=>
                {
                    slide.SetActive(false);
                });
            }
        }

        public void AnimateEntrance()
        {
            fadeImage.gameObject.SetActive(true);
            if(_fadeTween.isAlive) _fadeTween.Stop();
            _fadeTween = Tween.Alpha(fadeImage, 1f, 0f, fadeDuration ).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
        }

        public void AnimateExit()
        {
            fadeImage.gameObject.SetActive(true);
            if(_fadeTween.isAlive) _fadeTween.Stop();
            fadeImage.gameObject.SetActive(true);
            _fadeTween = Tween.Alpha(fadeImage, 0f, 1f, fadeDuration).OnComplete(() =>
            {
                
            });
        }
        
        

        public void GetCurrentSlides()
        {
            switch (_currentLanguage)
            {
                case GameManager.GameLanguage.Spanish:
                    _currentSlide = casSlides[_currentSlideIndex];
                    _currentSlides = casSlides;
                    break;
                case GameManager.GameLanguage.English:
                    _currentSlide = engSlides[_currentSlideIndex];
                    _currentSlides = engSlides;
                    break;
                case GameManager.GameLanguage.Catalan:
                default:
                    _currentSlide = catSlides[_currentSlideIndex];
                    _currentSlides = catSlides;
                    break;
            }
        }
    }
}
