using System;
using Global.QuestionManagers;
using UnityEngine;
using Global.Types;
using Unity.VisualScripting;

namespace Global
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset defaultQuestionsFile;
        [SerializeField] private TextAsset spanishQuestionsFile;
        [SerializeField] private TextAsset englishQuestionsFile;
        
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
        
        private void Start()
        {
            _defaultGameData = LevelParsing.ParseAllQuestions(defaultQuestionsFile.text);
            _spanishGameData = LevelParsing.ParseAllQuestions(spanishQuestionsFile.text);
            _englishGameData = LevelParsing.ParseAllQuestions(englishQuestionsFile.text);
            
            _currentGameData = _defaultGameData;

            _currentLevelIndex = 0;
            _currentQuestionIndex = 0;
        }


        private void LoadLevel()
        {
              _currentLevel = _currentGameData.Levels[_currentLevelIndex];
        }

        private void LoadQuestion(BaseQuestion question)
        {
            _currentQuestion = question;
            var t = GetCurrentManager();
            
            if (_currentManager != t)
            {
                DisableOtherManagers(t);
            }
            
            GameEvents.LoadQuestion?.Invoke(_currentQuestion);
        }
        
        private BaseQuestionManager GetCurrentManager()
        {
            BaseQuestionManager manager = null;
            
            if (_currentQuestion is MultiChoiceQuestion)
            {
                manager = multiChoiceManager;
            }
            else if (_currentQuestion is DragAndDropQuestion)
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
                    dragNDropManager.gameObject.SetActive(false);
                if (activeManager != multiChoiceManager)
                    multiChoiceManager.gameObject.SetActive(false);
            }
        }
        
        private void NextQuestion()
        {
            _currentQuestionIndex++;

            if (_currentLevelIndex >= _currentLevel.Questions.Count)
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
                LoadLevel();
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
                    break;
                case GameLanguage.Spanish:
                    _currentGameData = _spanishGameData;
                    break;
                case GameLanguage.English:
                    _currentGameData = _englishGameData;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
            
            GameEvents.LanguageChanged?.Invoke();
        }
        
        public enum GameLanguage
        {
            Catalan,
            Spanish,
            English
        }
    }
}