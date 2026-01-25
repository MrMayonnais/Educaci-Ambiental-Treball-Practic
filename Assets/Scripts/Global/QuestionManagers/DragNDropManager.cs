using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Global;
using Global.QuestionManagers;
using Unity.VisualScripting;

namespace Global.QuestionManagers
{
    public class DragNDropManager : BaseQuestionManager
    {
        [Header("UI References")] public GameObject questionPanel;
        public GameObject feedbackPanel;
        public GameObject nextButton;
        public GameObject restartButton;

        [Header("Questions")] 
        public GameObject layout1;
        public GameObject layout2;
        public GameObject layout3;
        public GameObject layout4;

        [Header("Feedback Sprites")] public Sprite correctSprite;
        public Sprite incorrectSprite;
        
        [Header("Animation Clips")]
        public AnimationClip displayLayout0Clip;
        public AnimationClip displayLayout1Clip;
        public AnimationClip displayLayout2Clip;
        public AnimationClip displayLayout3Clip;
        public AnimationClip showCorrectFeedbackClip;
        public AnimationClip showIncorrectFeedbackClip;

        private DragAndDropQuestion _currentQuestion;
        
        private Dictionary<DropZone, List<DraggableItem>> _currentCorrectMatches;
        private GameObject _currentLayout;
        private List<DraggableItem> _draggableItems = new List<DraggableItem>();
        private List<DropZone> _dropZones = new List<DropZone>();

        private Button _nextButton;
        private Button _restartButton;
        private TextMeshProUGUI _questionText;
        private Button _questionButton;
        private TextMeshProUGUI _feedbackText;
        private Animator _animator;
        
        private Animation _animation;

        private int _correctDrops;


        private void OnEnable()
        {
            DraggableItem.OnItemDropped += OnItemDropped;
            GameEvents.LoadQuestion += LoadQuestion;
        }

        private void OnDisable()
        {
            DraggableItem.OnItemDropped -= OnItemDropped;
            GameEvents.LoadQuestion -= LoadQuestion;
        }

        private void Start()
        {
            _animation = GetComponent<Animation>();
            if (!_animation)
            {
                _animation = gameObject.AddComponent<Animation>();
            }
            

            // Add clips to the animation component - nombres consistentes con DisplayQuestion
            if (displayLayout0Clip)
            {
                _animation.AddClip(displayLayout0Clip, "DisplayLayout1");
            }

            if (displayLayout1Clip)
            {
                _animation.AddClip(displayLayout1Clip, "DisplayLayout2");
            }
    
            if (displayLayout2Clip)
            {
                _animation.AddClip(displayLayout2Clip, "DisplayLayout3");
            }
    
            if (displayLayout3Clip)
            {
                _animation.AddClip(displayLayout3Clip, "DisplayLayout4");
            }
    
            if (showCorrectFeedbackClip) _animation.AddClip(showCorrectFeedbackClip, "ShowCorrectFeedback");
            if (showIncorrectFeedbackClip) _animation.AddClip(showIncorrectFeedbackClip, "ShowIncorrectFeedback");

            _nextButton = nextButton.GetComponent<Button>();
            _restartButton = restartButton.GetComponent<Button>();
            _questionText = questionPanel.GetComponentInChildren<TextMeshProUGUI>();
            _questionButton = questionPanel.GetComponentInChildren<Button>();
            _feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();

            _nextButton.onClick.AddListener(OnNextButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _questionButton.onClick.AddListener(OnQuestionButtonClicked);
        }

        public override void LoadQuestion(BaseQuestion question)
        {
            
        }
        
        public void DisplayQuestion(int questionN)
        {
            _currentLayout = questionN switch
            {
                0 => layout1,
                1 => layout2,
                2 => layout3,
                3 => layout4,
                _ => layout1
            };
            
            var clipName = $"DisplayLayout{questionN+1}";
            if (_animation.GetClip(clipName))
            {
                _animation.Play(clipName);
            }
            
            _currentLayout.SetActive(true);
            feedbackPanel.SetActive(false);
            nextButton.SetActive(false);
            restartButton.SetActive(false);
            questionPanel.SetActive(true);
            
            DisableOtherLayouts();
            GetCorrectMatches();
            ResetQuestionItems();
        }
        
        private void GetCorrectMatches()
        {
            _dropZones = _currentLayout.GetComponentsInChildren<DropZone>().ToList();
            _draggableItems = _currentLayout.GetComponentsInChildren<DraggableItem>().ToList();
            _currentCorrectMatches = new Dictionary<DropZone, List<DraggableItem>>();

            foreach (var dropZone in _dropZones)
            {
                _currentCorrectMatches.Add(dropZone, dropZone.GetCorrectItems());
                Debug.Log("DropZone: " + dropZone.name + " has " + _currentCorrectMatches[dropZone].Count + " correct items.");
            }
        }

        private void ResetQuestionItems()
        {
            foreach (var draggableItem in _draggableItems)
            {
                GameEvents.ForceItemReturn?.Invoke(draggableItem);
            }

            foreach (var dropZone in _dropZones)
            {
                var item = dropZone.GetComponentInChildren<DraggableItem>();
                if (item)
                {
                    GameEvents.ForceItemReturn?.Invoke(item);
                }
                
                GameEvents.ForceDisappearDropZoneImage?.Invoke(dropZone);
            }
        }

        private void OnItemDropped(DraggableItem item, DropZone zone)
        {
            Debug.Log("Dropped on drop zone " + zone.name);
            
            if(!_currentCorrectMatches[zone].Contains(item))
            { 
                Debug.Log("Incorrect drop detected---current_value: " + _correctDrops + "---total_needed: " + _currentCorrectMatches.Count);
                DisplayIncorrectFeedback();
            }
            else
            {
                Debug.Log("Correct drop detected---updating state---current_value: " + _correctDrops+1+ "---total_needed: " + _currentCorrectMatches.Count);
                _correctDrops++;

                GameEvents.AppearDropZoneImage?.Invoke(zone);
        
                if (_correctDrops >= _currentCorrectMatches.Count)
                {
                    DisplayCorrectFeedback();
                }
            }
            
            Debug.Log("Current correct drops: " + _correctDrops + " out of " + _currentCorrectMatches.Count);
        }
        
        private void DisplayIncorrectFeedback()
        {
            feedbackPanel.GetComponent<Image>().sprite = incorrectSprite;
            
            if (_animation.GetClip("ShowIncorrectFeedback") != null)
            {
                _animation.Play("ShowIncorrectFeedback");
            }
            
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(true);
            restartButton.SetActive(true);
        }
        
        private void DisplayCorrectFeedback()
        {
            feedbackPanel.GetComponent<Image>().sprite = correctSprite;
            
            if (_animation.GetClip("ShowCorrectFeedback") != null)
            {
                _animation.Play("ShowCorrectFeedback");
            }
            
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(true);
            nextButton.SetActive(true);
        }
        
        private void DisableOtherLayouts()
        {
            foreach (var layout in new List<GameObject> {layout1, layout2, layout3, layout4})
            {
                if (layout != _currentLayout)
                    layout.SetActive(false);
            }
        }
        
        private void OnNextButtonClicked()
        {
            _currentCorrectMatches = new Dictionary<DropZone, List<DraggableItem>>();
            _correctDrops = 0;
            GameEvents.OnNextQuestion?.Invoke();
        }
        
        private void OnRestartButtonClicked()
        {
            _currentCorrectMatches = new Dictionary<DropZone, List<DraggableItem>>();
            _correctDrops = 0;
            GameEvents.OnRestartLevel?.Invoke();    
        }
        
        private void OnQuestionButtonClicked()
        {
            questionPanel.SetActive(false);
        }
        
        public void ActivateQuestionButton()
        {
            _questionButton.interactable = true;
        }

        public void DeactivateQuestionButton()
        {
            _questionButton.interactable = false;
        }

        public void DeactivateControlButtons()
        {
            _nextButton.interactable = false;
            _restartButton.interactable = false;
        }
        
        public void ActivateControlButtons()
        {
            _nextButton.interactable = true;
            _restartButton.interactable = true;
        }
        
    }
}
