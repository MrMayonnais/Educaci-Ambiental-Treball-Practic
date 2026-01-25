using System.Collections.Generic;
using Global.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Global.QuestionManagers
{
    public class NewDragNDropManager: BaseQuestionManager
    {
        [Header("UI References")]
        public GameObject questionPanel;
        public GameObject feedbackPanel;
        public GameObject restartButton;
        public GameObject nextButton;
        public List<GameObject> layouts;
        
        [Header("Animations")]
        public List<AnimationClip> displayLayoutClips;
        
        public AnimationClip showCorrectFeedbackClip;
        public AnimationClip showIncorrectFeedbackClip;
        
        [Header("Feedback Sprites")]
        public Sprite correctSprite;
        public Sprite incorrectSprite;
        
        // Private UI references//
        private TextMeshProUGUI _questionText;
        private TextMeshProUGUI _feedbackText;
        
        private Button _questionContinueButton;
        private Button _nextButton;
        private Button _restartButton;
        
        private List<DraggableItem> _draggableItems;
        private List<DropZone> _dropZones;
        
        private Animation _animation;
        //-------------------//
        
        private DragAndDropQuestion _currentQuestion;
        private List<CorrectMatch> _playerMatches;

        private void Start()
        {
            LoadAnimations();   
            GetComponents();
        }
        
        private void LoadAnimations()
        {
            _animation = GetComponent<Animation>();
            if (!_animation)
            {
                _animation = gameObject.AddComponent<Animation>();
            }
            
            for (int i = 0; i < displayLayoutClips.Count; i++)
            {
                var clip = displayLayoutClips[i];
                if (clip)
                {
                    _animation.AddClip(clip, "DisplayLayout" + i + 1);
                }
            }
            
            if (showCorrectFeedbackClip) _animation.AddClip(showCorrectFeedbackClip, "ShowCorrectFeedback");
            if (showIncorrectFeedbackClip) _animation.AddClip(showIncorrectFeedbackClip, "ShowIncorrectFeedback");
        }

        private void GetComponents()
        {
            _nextButton = nextButton.GetComponent<Button>();
            _restartButton = restartButton.GetComponent<Button>();
            _questionText = questionPanel.GetComponentInChildren<TextMeshProUGUI>();
            _questionContinueButton = questionPanel.GetComponentInChildren<Button>();
            _feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();

            _nextButton.onClick.AddListener(OnNextButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _questionContinueButton.onClick.AddListener(OnQuestionButtonClicked);
        }
        
        private static void OnNextButtonClicked()
        {
            GameEvents.OnNextQuestion?.Invoke();
        }   
        
        private static void OnRestartButtonClicked()
        {
            GameEvents.OnRestartLevel?.Invoke();
        }

        private static void OnQuestionButtonClicked()
        {
            //TODO: Implement if needed
        }
    }
}