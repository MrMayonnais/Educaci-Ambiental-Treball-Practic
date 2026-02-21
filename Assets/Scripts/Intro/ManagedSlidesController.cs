using System.Collections.Generic;
using Dlcs;
using Global;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Intro
{
    public class ManagedSlidesController : MonoBehaviour, ISlidesController
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

        public float subtitlesPanelDisplacement = 200f;
        public float continueButtonDisplacement = 100f;
        
        public float fadeDuration = 1f;
        public float slideTransitionDuration = 1f;
        
        public float subtitlesPanelTransitionDuration = 0.5f;
        public float continueButtonTransitionDuration = 0.5f;

        private float _startButtonX;
        private float _normalButtonX;
        private float _exitButtonX;
        
        
        private Button _continueButton;
        
        private int _currentSlideIndex = 0;
        
        private List<GameObject> _currentSlides;
        private GameObject _currentSlide;
        private GameObject _previousSlide;
        
        private Tween _fadeTween;
        private Tween _enterSlideTween;
        private Tween _exitSlideTween;
        private Tween _subtitlesPanelTween;
        private Tween _continueButtonTween;
        
        private GameManager.GameLanguage _currentLanguage = GameManager.GameLanguage.Catalan;
        
        private void Awake()
        {
            _continueButton = subtitles.gameObject.GetChildByNameRecursively("ContinueButton").GetComponent<Button>();
            _continueButton.onClick.AddListener(AnimateExit);
        }
        
        private void Start()
        {
            
            
            _currentLanguage = LanguageChanger.CurrentLanguage();
            GetCurrentSlides();
            _currentSlideIndex = -1;
            
            _startButtonX = _continueButton.transform.position.x- continueButtonDisplacement;
            _normalButtonX = _continueButton.transform.position.x;
            _exitButtonX = _continueButton.transform.position.x + continueButtonDisplacement;
        }
        
        private void OnEnable()
        {
            GameEvents.LanguageChanged += ChangeLanguage;
            GameEvents.OnAppearSlide += NextSlide;

        }
        
        private void OnDisable()
        {
            GameEvents.LanguageChanged -= ChangeLanguage;
            GameEvents.OnAppearSlide -= NextSlide;
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
            }
        }

        public void LoadSlide(GameObject slide)
        {
            AnimateEntrance();
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
            Debug.Log("Animating Entrance");
            
            GameEvents.PlayIntroMusic?.Invoke();
            
            GameEvents.ChangeSlideBackground?.Invoke();
            
            fadeImage.gameObject.SetActive(true);
            
            if(_fadeTween.isAlive) _fadeTween.Stop();
            _fadeTween = Tween.Alpha(fadeImage, 1f, 0f, fadeDuration ).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
            
            
            subtitles.gameObject.SetActive(true);
            var initialPosition = subtitles.transform.position;
            
            if(_subtitlesPanelTween.isAlive) _enterSlideTween.Stop();
            _subtitlesPanelTween = Tween.PositionX(subtitles.transform,
                initialPosition.x - subtitlesPanelDisplacement,
                initialPosition.x, subtitlesPanelTransitionDuration
                ,Easing.Standard(Ease.InOutCubic));
            
            
            _continueButton.gameObject.SetActive(true);
            _continueButton.interactable = false;
            var buttonInitialPosition = _continueButton.transform.position;
            
            if(_continueButtonTween.isAlive) _exitSlideTween.Stop();
            _continueButtonTween = Tween.PositionX(_continueButton.transform,
                _startButtonX,
                _normalButtonX, continueButtonTransitionDuration,
                Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                _continueButton.interactable = true;
            });
        }

        public void AnimateExit()
        {
            AnimateSlideExit(_previousSlide);

            _previousSlide = null;
            subtitles.CancelTyping();
            dialoguePlayer.StopCurrentClip();
            
            fadeImage.gameObject.SetActive(true);
            if(_fadeTween.isAlive) _fadeTween.Stop();
            fadeImage.gameObject.SetActive(true);
            _fadeTween = Tween.Alpha(fadeImage, 0f, 1f, fadeDuration).OnComplete(() =>
            {
                GameEvents.OnStartLevel?.Invoke();
            });
            
            subtitles.gameObject.SetActive(true);
            var initialPosition = subtitles.transform.position;
            
            if(_subtitlesPanelTween.isAlive) _enterSlideTween.Stop();
            _subtitlesPanelTween = Tween.PositionX(subtitles.transform,
                initialPosition.x,
                initialPosition.x + subtitlesPanelDisplacement, subtitlesPanelTransitionDuration,
                Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                subtitles.gameObject.SetActive(false);
                subtitles.transform.position = initialPosition;
            });
            
            
            _continueButton.interactable = false;
            var initialButtonPosition = _continueButton.transform.position;
            
            if(_continueButtonTween.isAlive) _exitSlideTween.Stop();
            _continueButtonTween = Tween.PositionX(_continueButton.transform,
                _normalButtonX,
                _exitButtonX, continueButtonTransitionDuration, 
                Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                _continueButton.gameObject.SetActive(false);
                _continueButton.transform.position = initialButtonPosition;
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
