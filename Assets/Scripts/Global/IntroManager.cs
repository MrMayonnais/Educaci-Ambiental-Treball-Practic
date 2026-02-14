using System.Collections.Generic;
using UnityEngine;

using Global;
using PrimeTween;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dlcs
{
    public class IntroManager: MonoBehaviour
    {
        public bool selfControlled = true;
        
        public DialoguePlayer clipPlayer;
        public SubtitlesController subtitles;
        
        public List<GameObject> catSlides;
        public List<GameObject> castSlides;
        public List<GameObject> engSlides;

        public Button continueButton;
        public Image fadeImage;
        
        public float slideAnimationDuration = 0.5f;
        public float slideAnimationDisplacement = 1000f;
        
        public float entranceAnimationDuration = 2f;
        
        private List<GameObject> _currentSlides;
        
        private int _currentSlideIndex = 0;
        private bool _isShowingSlide = false;
        
        private Tween _currentTween;

        private Tween _nextSlideTween;
        private Tween _previousSlideTween;
        

        private void OnEnable()
        {
            GameEvents.LanguageChanged += ChangeLanguage;
        }

        private void OnDisable()
        {
            GameEvents.LanguageChanged -= ChangeLanguage;
        }
        
        private void Awake()
        {
            _currentSlides?[_currentSlideIndex].SetActive(false);

            switch (LanguageChanger.CurrentLanguage())
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
        }
        
        private void Start()
        {
            if(selfControlled) AnimateEntrance();
        }

        public void PlayNextSlide()
        {
            AnimateEntrance();
        }


        private void NextSlide()
        {
            _currentSlideIndex++;
            
            clipPlayer.StopCurrentClip();
            subtitles.CancelTyping();

            if (selfControlled)
            {
                if (_currentSlideIndex >= _currentSlides.Count)
                {
                    EndScene();
                    return;
                }
                LoadSlide(_currentSlideIndex);
            }
            else
            {
                StartLevelAnimated();
            }
        }   
        
        private void LoadSlide(int index)
        {
            _currentSlides[index].SetActive(true);
            
            continueButton.interactable = false;
            
            AnimateSlideChange();
            
            var clipToPlay = _currentSlides[index].GetComponent<AudioSource>();
            if (clipToPlay)
            {
                clipPlayer.PlayClip(clipToPlay.clip);
            }

            _isShowingSlide = true;
        }

        private void ChangeLanguage(GameManager.GameLanguage language)
        {
            if (_isShowingSlide)
            {
                _currentSlides?[_currentSlideIndex].SetActive(false);

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

        private void AnimateSlideChange()
        {
            subtitles.StopTyping();
            
            subtitles.gameObject.SetActive(true);
            Debug.Log("Subtitles now active");
            
            if (_currentSlideIndex >= 1)
            {
                var originalPosition = _currentSlides[_currentSlideIndex-1].transform.position;
                
                _previousSlideTween = Tween.PositionX(_currentSlides[_currentSlideIndex-1].transform, originalPosition.x,originalPosition.x + slideAnimationDisplacement, slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                {
                    _currentSlides[_currentSlideIndex-1].SetActive(false);
                    _currentSlides[_currentSlideIndex-1].transform.position = originalPosition;
                });
            }
            
            var originalPosition2 = _currentSlides[_currentSlideIndex].transform.position;
            
            _nextSlideTween = Tween.PositionX(_currentSlides[_currentSlideIndex].transform, originalPosition2.x-slideAnimationDisplacement, originalPosition2.x, slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                subtitles.PlaySubtitles(_currentSlideIndex);
                _currentSlides[_currentSlideIndex].transform.position = originalPosition2;
                continueButton.interactable = true;
            });
            
        }
        
        private void AnimateEntrance()
        {
            Debug.Log("Animating entrance");
            fadeImage.gameObject.SetActive(true);
            _currentTween = Tween.Color(fadeImage, startValue: new Color(0,0,0,1),endValue: new Color(0,0,0,0), duration: entranceAnimationDuration).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
                _isShowingSlide = true;
                ChangeLanguage(LanguageChanger.CurrentLanguage());
                continueButton.onClick.AddListener(NextSlide);
            });
        }
        
        private void EndScene()
        {
            fadeImage.gameObject.SetActive(true);
            
            _currentTween.Stop();
            _currentTween = Tween.Color(fadeImage, startValue: new Color(0,0,0,0), endValue: new Color(0,0,0,1), duration: 1f).OnComplete(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            });
        }

        private void SlideExit()
        {
            _currentTween.Stop();
            _currentTween = Tween.Color(fadeImage, startValue: new Color(0,0,0,0), endValue: new Color(0,0,0,1), duration: 1f).OnComplete(() =>
            {
                subtitles.gameObject.SetActive(false);
                
                
            });
        }

        private void StartLevelAnimated()
        {
            var originalPosition = _currentSlides[_currentSlideIndex].transform.position;
            var subOriginalPosition = subtitles.transform.position;
            var buttonOriginalPosition = continueButton.transform.position;

            _currentTween = Tween.PositionX(subtitles.transform,
                subOriginalPosition.x + slideAnimationDisplacement,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                {
                    subtitles.gameObject.SetActive(false);
                    subtitles.transform.position = originalPosition;
                });
            
            continueButton.interactable = false;
            
            _previousSlideTween = Tween.PositionX(continueButton.transform,
                buttonOriginalPosition.x + slideAnimationDisplacement*30,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                {
                    continueButton.gameObject.SetActive(false); 
                    continueButton.transform.position = buttonOriginalPosition;
                    continueButton.interactable = true;
                });
            
            _nextSlideTween = Tween.PositionY(_currentSlides[_currentSlideIndex].transform, 
                originalPosition.y - slideAnimationDisplacement,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                {
                    _currentSlides[_currentSlideIndex].SetActive(false); 
                    _currentSlides[_currentSlideIndex].transform.position = originalPosition; 
                    
                    GameEvents.OnStartLevel?.Invoke();
                });

            _isShowingSlide = false;

        }

        private void ReturnToIntro()
        {
            var originalPosition = _currentSlides[_currentSlideIndex].transform.position;
            var subOriginalPosition = subtitles.transform.position;
            var buttonOriginalPosition = continueButton.transform.position;
            
            _currentSlides[_currentSlideIndex].SetActive(true);
            subtitles.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);

            _currentTween = Tween.PositionX(subtitles.transform,
                subOriginalPosition.x + slideAnimationDisplacement,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                
            });
            
            continueButton.interactable = false;
            
            _previousSlideTween = Tween.PositionX(continueButton.transform,
                buttonOriginalPosition.x + slideAnimationDisplacement*30,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                
            });
            
            _nextSlideTween = Tween.PositionY(_currentSlides[_currentSlideIndex].transform, 
                originalPosition.y + slideAnimationDisplacement,
                slideAnimationDuration, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                    
                
            });

            _isShowingSlide = false;
        }
    }
}