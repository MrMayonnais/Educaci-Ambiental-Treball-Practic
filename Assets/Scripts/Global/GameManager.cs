using System;
using Dlcs;
using Global.QuestionManagers;
using UnityEngine;
using Global.Types;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Global
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset defaultQuestionsFile;
        [SerializeField] private TextAsset spanishQuestionsFile;
        [SerializeField] private TextAsset englishQuestionsFile;
        
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
        public IntroManager introManager;
        
        private BaseQuestionManager _currentManager;
        
        private void Start()
        {
            _defaultGameData = LevelParsing.ParseAllQuestions(defaultQuestionsFile.text);
            if(spanishQuestionsFile)_spanishGameData = LevelParsing.ParseAllQuestions(spanishQuestionsFile.text);
            if(englishQuestionsFile)_englishGameData = LevelParsing.ParseAllQuestions(englishQuestionsFile.text);
            
            _currentGameData = _defaultGameData;

            _currentLevelIndex = startLevel;
            _currentQuestionIndex = startQuestion;
            
            introManager.PlayNextSlide();
            
        }

        private void StartLevel()
        {
            LoadLevel();
            LoadQuestion(_currentLevel.Questions[_currentQuestionIndex]);
        }

        private void OnEnable()
        {
            GameEvents.OnNextQuestion += NextQuestion;
            GameEvents.OnRestartLevel += RestartLevel;
            //GameEvents.LanguageChanged += ChangeLanguageTo;
            GameEvents.OnStartLevel += StartLevel;
        }
        
        private void OnDisable()
        {
            GameEvents.OnNextQuestion -= NextQuestion;
            GameEvents.OnRestartLevel -= RestartLevel;
            //GameEvents.LanguageChanged -= ChangeLanguageTo;
            GameEvents.OnStartLevel -= StartLevel;
        }

        private void LoadLevel()
        {
            _currentLevel = _currentGameData.Levels[_currentLevelIndex];
            PrintLevelQuestions(_currentLevel);
        }

        private void LoadQuestion(BaseQuestion question)
        {
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
                    Debug.Log("Disabling DragNDropManager");
                }

                if (activeManager != multiChoiceManager)
                {
                    multiChoiceManager.gameObject.SetActive(false);
                    Debug.Log("Disabling MultiChoiceManager");
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
                
                var originalPosition = _currentManager.transform.position;
                Tween.PositionY(_currentManager.transform,
                    originalPosition.y,
                    originalPosition.y + Screen.height,
                    2f, Easing.Standard(Ease.InOutCubic)).OnComplete(() =>
                    {
                        introManager.PlayNextSlide();
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
        
        private void PrintLevelQuestions(LevelData level)
        {
            Debug.Log($"=== Level {level.LevelNumber} ===");
            Debug.Log($"Total Questions: {level.Questions.Count}");

            foreach (var question in level.Questions)
            {
                Debug.Log($"\nQuestion {question.QuestionNumber}:");

                if (question is MultiChoiceQuestion mcq)
                {
                    Debug.Log($"  Type: Multiple Choice");
                    Debug.Log($"  Question: {mcq.QuestionText}");
                    Debug.Log($"  Options: a) {mcq.OptionA}, b) {mcq.OptionB}, c) {mcq.OptionC}, d) {mcq.OptionD}");
                    Debug.Log($"  Correct Answer: {mcq.CorrectAnswer}");
                }
                else if (question is DragAndDropQuestion dnd)
                {
                    Debug.Log($"  Type: Drag and Drop");
                    Debug.Log($"  Question: {dnd.QuestionText}");
                    Debug.Log($"  Draggables: {dnd.DraggableItems.Count}, DropZones: {dnd.DropZones.Count}");
                    Debug.Log($"  Correct Matches: {dnd.CorrectMatches.Count}");
                }

                Debug.Log("---");
            }
        }
    }
}