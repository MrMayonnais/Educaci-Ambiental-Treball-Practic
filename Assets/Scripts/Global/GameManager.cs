using System;
using Dlcs;
using Global.QuestionManagers;
using UnityEngine;
using Global.Types;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Global
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset defaultQuestionsFile;
        [SerializeField] private TextAsset spanishQuestionsFile;
        [SerializeField] private TextAsset englishQuestionsFile;
        
        public Image fadeImage;
        
        public int startLevel = 1;
        public int startQuestion = 0;
        
        private GameData _defaultGameData;
        private GameData _spanishGameData;
        private GameData _englishGameData;
        
        private GameData _currentGameData;
        
        private LevelData _currentLevel;
        private int _currentLevelIndex;
        
        private BaseQuestion _currentQuestion;
        private int _currentQuestionIndex;
        
        public DragNDropManager dragNDropManager;
        public MultiChoiceManager multiChoiceManager;
        
        private BaseQuestionManager _currentManager;
        
        private Tween _fadeTween;
        
        private void Start()
        {
            _defaultGameData = LevelParsing.ParseAllQuestions(defaultQuestionsFile.text);
            if(spanishQuestionsFile)_spanishGameData = LevelParsing.ParseAllQuestions(spanishQuestionsFile.text);
            if(englishQuestionsFile)_englishGameData = LevelParsing.ParseAllQuestions(englishQuestionsFile.text);
            
            _currentGameData = _defaultGameData;

            _currentLevelIndex = startLevel;
            _currentQuestionIndex = startQuestion;
            _currentLevel = _currentGameData.Levels[_currentLevelIndex];
            
            Begin();
            
        }

        private void Begin()
        {
            AnimateEntrance();
        }

        private void StartLevel()
        {
            LoadLevel();
            AnimateSlideExit();
        }

        private void OnEnable()
        {
            GameEvents.OnNextQuestion += NextQuestion;
            GameEvents.OnRestartLevel += RestartLevel;
            GameEvents.LanguageChanged += ChangeLanguageTo;
            GameEvents.OnStartLevel += StartLevel;
            GameEvents.OnNextLevel += NextLevel;
        }
        
        private void OnDisable()
        {
            GameEvents.OnNextQuestion -= NextQuestion;
            GameEvents.OnRestartLevel -= RestartLevel;
            GameEvents.LanguageChanged -= ChangeLanguageTo;
            GameEvents.OnStartLevel -= StartLevel;
            GameEvents.OnNextLevel -= NextLevel;
        }

        private void LoadLevel()
        {
            _currentLevel = _currentGameData.Levels[_currentLevelIndex];
            LoadQuestion(_currentLevel.Questions[_currentQuestionIndex]);
        }

        private void LoadQuestion(BaseQuestion question)
        {
            Debug.Log("Loading Question: " + question.QuestionText+ " with type " + question.Type);
            _currentQuestion = question;
            
            var t = GetCurrentManager();
            
            if (_currentManager != t || !_currentManager)
            {
                DisableOtherManagers(t);
                _currentManager = t;
                _currentManager.gameObject.SetActive(true);
            }
            
            GameEvents.LoadQuestion?.Invoke(_currentQuestion);
        }
        
        private BaseQuestionManager GetCurrentManager()
        {
            BaseQuestionManager manager = null;
            
            if (_currentQuestion.Type == QuestionType.MultipleChoice)
            {
                manager = multiChoiceManager;
            }
            else if (_currentQuestion.Type == QuestionType.DragAndDrop)
            {
                manager = dragNDropManager;
            }

            return manager;
        }

        private void DisableOtherManagers(BaseQuestionManager activeManager)
        {
            if (activeManager != null)
            {
                if (activeManager != dragNDropManager)
                {
                    dragNDropManager.gameObject.SetActive(false);
                }

                if (activeManager != multiChoiceManager)
                {
                    multiChoiceManager.gameObject.SetActive(false);
                }
            }
        }
        
        private void NextQuestion()
        {
            _currentQuestionIndex++;
            
            
            if (_currentQuestionIndex>= _currentLevel.Questions.Count)
            {
                _currentQuestionIndex = 0;
                
                NextLevel();
            }
            else
            {
                LoadQuestion(_currentLevel.Questions[_currentQuestionIndex]);
            }
        }
        
        private void RestartLevel()
        {
            _currentQuestionIndex = 0;
            LoadQuestion(_currentLevel.Questions[_currentQuestionIndex]);
        }

        private void NextLevel()
        {
            _currentLevelIndex++;
    
            if (_currentLevelIndex >= _defaultGameData.Levels.Count)
            {
                EndGame();
            }
            else
            {
                if (_currentManager == null)
                {
                    Debug.LogWarning("No current manager found.");
                    return;
                }
        
                var originalPosition = _currentManager.transform.position;
                Tween.PositionY(_currentManager.transform,
                    originalPosition.y,
                    originalPosition.y + Screen.height,
                    2f, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                {
                    _currentQuestionIndex = 0;
                    AnimateExit();
                });
            }
        }

        private void EndGame()
        {
            Debug.Log("Game Over!");
        }

        private void ChangeLanguageTo(GameLanguage language)
        {
            switch (language)
            {
                case GameLanguage.Catalan:
                    _currentGameData = _defaultGameData;
                    GameEvents.LanguageChanged?.Invoke(GameLanguage.Catalan);
                    break;
                case GameLanguage.Spanish:
                    _currentGameData = _spanishGameData;
                    GameEvents.LanguageChanged?.Invoke(GameLanguage.Spanish);
                    break;
                case GameLanguage.English:
                    _currentGameData = _englishGameData;
                    GameEvents.LanguageChanged?.Invoke(GameLanguage.English);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
            
            
        }
        
        public enum GameLanguage
        {
            Catalan,
            Spanish,
            English
        }
        

        private void AnimateExit()
        {
            fadeImage.gameObject.SetActive(true);
            if(_fadeTween.isAlive) _fadeTween.Stop();
            _fadeTween = Tween.Alpha(fadeImage, 0, 1, 1f, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                GameEvents.OnAppearSlide?.Invoke();
            });
        }

        private void AnimateSlideExit()
        {
            GameEvents.PlayGameMusic ?.Invoke();
            GameEvents.ChangeQuestionBackground?.Invoke();
            
            
            if (_fadeTween.isAlive) _fadeTween.Stop();
            _fadeTween = Tween.Alpha(fadeImage, 
                1f, 0f, .5f, 
                Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
        }

        private void AnimateEntrance()
        {
            if(_fadeTween.isAlive) _fadeTween.Stop();
            _fadeTween = Tween.Alpha(fadeImage, 1, 0, .5f, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
                GameEvents.OnAppearSlide?.Invoke();
            });
        }
    }
}