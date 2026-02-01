using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Global.QuestionManagers
{
    public class MultiChoiceManager : BaseQuestionManager
    {
        private static readonly int DisplayAnimation = Animator.StringToHash("Display");
        
        [Header("UI References")]
        
        public GameObject questionPanel;
        public GameObject feedbackPanel;
        
        public Button nextButton;
        public Button restartButton;
    
        public GameObject optionA;
        public GameObject optionB;
        public GameObject optionC;
        public GameObject optionD;
        
        [Header("Feedback Sprites")]
        public Sprite correctSprite;
        public Sprite incorrectSprite;
        
        
        // Private UI References //
        private TextMeshProUGUI _questionText;
        private TextMeshProUGUI _feedbackText;
        
        private Button _optionAButton;
        private Button _optionBButton;
        private Button _optionCButton;
        private Button _optionDButton;
        //---------------------//

        [SerializeField] private Animator _animator;
    
        private MultiChoiceQuestion _currentQuestion;
    
        void Start()
        {
            GetComponents();
            InitializeComponents();
        }

        private void OnEnable()
        {
            GameEvents.LoadQuestion += LoadQuestion;
        }
        
        private void OnDisable()
        {
            GameEvents.LoadQuestion -= LoadQuestion;
        }

        private void LoadQuestion(BaseQuestion question)
        {
            if (question is MultiChoiceQuestion multiChoice)
            {
                _currentQuestion = multiChoice;
                Debug.Log("loading multi choice question :" + question.QuestionText);
                Debug.Log("with correct answer: " + multiChoice.CorrectAnswer);
                DisplayQuestion();
            }
            else Debug.Log("WRONG QUESTION TYPE FOR CURRENT MULTI CHOICE QUESTION MANAGER");
        }
    
        void DisplayQuestion()
        {
        
            feedbackPanel.SetActive(false);
		
            restartButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("BaseState"))_animator.SetTrigger(DisplayAnimation);
        }
    
        void CheckAnswer(char selectedOption)
        {
        
            var isCorrect = selectedOption == _currentQuestion.CorrectAnswer;
		
            AnimateResult(isCorrect);
        }
    
        private void AnimateResult(bool result)
        {
            if(result)
            {
                feedbackPanel.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.CorrectFeedback;
                feedbackPanel.GetComponent<Image>().sprite = correctSprite;
                _animator.SetTrigger("ShowCorrectFeedback");
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                feedbackPanel.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.IncorrectFeedback;
                feedbackPanel.GetComponent<Image>().sprite = incorrectSprite;
                _animator.SetTrigger("ShowIncorrectFeedback");
                restartButton.gameObject.SetActive(true);
            }
            feedbackPanel.SetActive(true);
        }
    
        private void OnNextButtonClicked()
        {
            GameEvents.OnNextQuestion?.Invoke();
        }
        
        private void OnRestartbuttonClicked()
        {
            GameEvents.OnRestartLevel?.Invoke();
        }

        private void GetComponents()
        {
            _optionAButton = optionA.GetComponentInChildren<Button>();
            _optionBButton = optionB.GetComponentInChildren<Button>();
            _optionCButton = optionC.GetComponentInChildren<Button>();
            _optionDButton = optionD.GetComponentInChildren<Button>();
        
            _questionText = questionPanel.GetComponentInChildren<TextMeshProUGUI>();
        
        
        
            _feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
    
        private void InitializeComponents()
        {
            _optionAButton.onClick.AddListener(() => CheckAnswer('a'));
            _optionBButton.onClick.AddListener(() => CheckAnswer('b'));
            _optionCButton.onClick.AddListener(() => CheckAnswer('c'));
            _optionDButton.onClick.AddListener(() => CheckAnswer('d'));
        
            nextButton.onClick.AddListener(OnNextButtonClicked);
            restartButton.onClick.AddListener(OnRestartbuttonClicked);
        
            nextButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
        
            feedbackPanel.SetActive(false);
        }

        private void SetAllText()
        {
            optionA.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.OptionA;
            optionB.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.OptionB;
            optionC.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.OptionC;
            optionD.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.OptionD;
        
            questionPanel.GetComponentInChildren<TextMeshProUGUI>().text = _currentQuestion.QuestionText;
            feedbackPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        
        private void DisableQuestionButtons()
        {
            _optionAButton.interactable = false;
            _optionBButton.interactable = false;
            _optionCButton.interactable = false;
            _optionDButton.interactable = false;
        
        }
    
        private void EnableQuestionButtons()
        {
            _optionAButton.interactable = true;
            _optionBButton.interactable = true;
            _optionCButton.interactable = true;
            _optionDButton.interactable = true;
        }
    
        private void DisableButtons()
        {
            restartButton.interactable = false;
            nextButton.interactable = false;
        }

        private void EnableButtons()
        {
            restartButton.interactable = true;
            nextButton.interactable = true;
        }
    }
}