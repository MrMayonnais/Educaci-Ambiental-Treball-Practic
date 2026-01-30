using System;
using Global.QuestionManagers;
using Global.Types;
using UnityEngine;

namespace Global
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Question Managers")]
        public MultiChoiceManager multiChoiceManager;
        public DragNDropManager dragNDropManager;
    
        [Header("Level Configuration")]
        public TextAsset level1File; // Archivo .txt para nivel 1
        public TextAsset level2File; // Archivo .txt para nivel 2
        public TextAsset level3File; // Archivo .txt para nivel 3
    
        private int _currentLevelIndex;
        private int _currentQuestionIndex;
    
        private LevelData _currentLevel;
        private BaseQuestion _currentQuestion;
    
        private BaseQuestionManager _currentManager;

        private const int LevelAmount = 3;

        private void Start()
        {
            _currentLevel = ParseLevel(_currentLevelIndex);
            _currentQuestion = _currentLevel.questions[0];
            LoadLevel();
        }

        private void OnEnable()
        {
            GameEvents.OnNextQuestion += NextQuestion;
            GameEvents.OnRestartLevel += RestartLevel;
        }

        private void OnDisable()
        {
            GameEvents.OnNextQuestion -= NextQuestion;
            GameEvents.OnRestartLevel -= RestartLevel;
        }

        private LevelData ParseLevel(int currentLevel = 0)
        {
            return currentLevel switch
            {
                0 => LevelParsing.ParseLevel1(level1File.text),
                1 => LevelParsing.ParseLevel2(level2File.text),
                2 => LevelParsing.ParseLevel3(level3File.text),
                _ => LevelParsing.ParseLevel1(level1File.text)
            };
        }

        private void LoadLevel()
        {
            _currentLevel = ParseLevel(); //level data now loaded locally
            _currentManager = GetCurrentManager();
            LoadCurrentQuestion();
        }	
        
        private void LoadQuestion(BaseQuestion question)
        {
            if(_currentManager is DragNDropManager _mL)
                _mL.DisplayQuestion(_currentQuestionIndex);
            else _currentManager.LoadQuestion(question);
        }
    
        private void LoadCurrentQuestion()
        {
            LoadQuestion(_currentQuestion);
        }
        
        private void NextQuestion()
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex < _currentLevel.questions.Count)
            {
                _currentQuestion = _currentLevel.questions[_currentQuestionIndex];
                LoadCurrentQuestion();
            }
                
            else
                NextLevel();
        }

        private void NextLevel()
        {
            _currentLevelIndex++;
            if (_currentLevelIndex < LevelAmount - 1)
            {
                LoadNextGame();
            }
            else
            {
                Debug.Log("No hay más niveles disponibles.");
            }
        }
    
        private void RestartLevel()
        {
            _currentQuestionIndex = 0;
            _currentQuestion = _currentLevel.questions[_currentQuestionIndex];
            LoadCurrentQuestion();
        }
        
        private void DeactivateAllManagers()
        {
            if (multiChoiceManager != null)
                multiChoiceManager.gameObject.SetActive(false);
            if (dragNDropManager != null)
                dragNDropManager.gameObject.SetActive(false);
        }
        
        private BaseQuestionManager GetCurrentManager()
        {
            DeactivateAllManagers();
            switch (_currentQuestion.Type)
            {
                case QuestionType.MultipleChoice:
                    multiChoiceManager.gameObject.SetActive(true);
                    return multiChoiceManager;
                case QuestionType.DragAndDrop:
                    dragNDropManager.gameObject.SetActive(true);
                    return dragNDropManager;
                default:
                    Debug.LogWarning("Tipo de pregunta no soportado: " + _currentQuestion.Type);
                    return null;
            }
        }

        private void LoadNextGame()
        {
            _currentLevel = ParseLevel(_currentLevelIndex);
            _currentQuestionIndex = 0;
            _currentQuestion = _currentLevel.questions[_currentQuestionIndex];
            _currentManager = GetCurrentManager();
            LoadCurrentQuestion();
        }
    }
}