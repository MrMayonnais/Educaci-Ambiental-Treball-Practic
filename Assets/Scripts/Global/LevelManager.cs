using System;
using UnityEngine;

namespace Global
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Question Managers")]
        public MultipleChoiceQuestionManager multiChoiceManager;
        //public DragQuestionManager dragQuestionManager;
    
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
            // Initialize level and question indices
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
            Debug.Log("Loading Question on "+_currentManager.name);
            _currentManager.LoadQuestion(question);
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
                //LoadMinigame();
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
                    // if (dragQuestionManager != null)
                    //     dragQuestionManager.gameObject.SetActive(false);
                }
        
        private void ActivateManagerForQuestion(BaseQuestion question)
                {
                    switch (question.type)
                    {
                        case QuestionType.MultipleChoice:
                            if (multiChoiceManager != null)
                                multiChoiceManager.gameObject.SetActive(true);
                            break;
                        
                        // case QuestionType.DragAndDrop4X4:
                        // case QuestionType.DragAndDrop7X7:
                        // case QuestionType.DragAndDrop9X9:
                        //     if (dragQuestionManager != null)
                        //         dragQuestionManager.gameObject.SetActive(true);
                        //     break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
        
        private BaseQuestionManager GetCurrentManager()
        {
            DeactivateAllManagers();
            ActivateManagerForQuestion(_currentQuestion);
            switch (_currentQuestion.type)
            {
                case QuestionType.MultipleChoice:
                    return multiChoiceManager;
                // case QuestionType.DragAndDrop4X4:
                // case QuestionType.DragAndDrop7X7:
                // case QuestionType.DragAndDrop9X9:
                //     return dragQuestionManager;
                default:
                    Debug.LogWarning("Tipo de pregunta no soportado: " + _currentQuestion.type);
                    return null;
            }
        }
    }
}