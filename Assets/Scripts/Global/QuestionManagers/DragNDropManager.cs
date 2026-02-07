using System.Collections.Generic;
using Global.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Global.QuestionManagers
{
    public class DragNDropManager: BaseQuestionManager
    {
        [Header("UI References")]
        public GameObject questionPanel;
        public GameObject feedbackPanel;
        public GameObject restartButton;
        public GameObject nextButton;
        public List<GameObject> layouts;
        
        [Header("Feedback Sprites")]
        public Sprite correctSprite;
        public Sprite incorrectSprite;
        
        // Private UI references//
        private GameObject _currentLayout;
        
        private TextMeshProUGUI _questionText;
        private TextMeshProUGUI _feedbackText;
        
        private Button _questionContinueButton;
        private Button _nextButton;
        private Button _restartButton;
        
        private List<DraggableItem> _draggableItems;
        private List<DropZone> _dropZones;
        
        private Animator _animator;
        //-------------------//
        
        private DragAndDropQuestion _currentQuestion;
        private List<CorrectMatch> _playerMatches = new List<CorrectMatch>();

        private void OnEnable()
        {
            GameEvents.LoadQuestion += LoadQuestion;
            DraggableItem.OnItemDropped += OnItemDropped;
        }

        private void OnDisable()
        {
            GameEvents.LoadQuestion -= LoadQuestion;
            DraggableItem.OnItemDropped -= OnItemDropped;
        }

        private void Awake()
        {
            GetComponents();
        }
        

        private void GetComponents()
        {
            _nextButton = nextButton.GetComponent<Button>();
            _restartButton = restartButton.GetComponent<Button>();
            _questionText = questionPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            _questionContinueButton = questionPanel.GetComponentInChildren<Button>(true);
            _feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            
            _animator = GetComponent<Animator>();

            _nextButton.onClick.AddListener(OnNextButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _questionContinueButton.onClick.AddListener(OnQuestionButtonClicked);
        }
        
        private void LoadQuestion(BaseQuestion question)
        {
            if(question is DragAndDropQuestion dragAndDropQuestion)
            {
                _currentQuestion = dragAndDropQuestion;
                _currentLayout = FindCurrentQuestionLayout();
                _currentLayout.SetActive(true);
                GetLayoutComponents();
                SetTexts();
                DisplayQuestion();
            }
            else Debug.Log("WRONG QUESTION TYPE FOR CURRENT DRAG N DROP QUESTION MANAGER");
        }
        
        private void GetLayoutComponents()
        {
            _draggableItems = new List<DraggableItem>();
            _dropZones = new List<DropZone>();
            
            var draggableItemsParent = Dlcs.Extensions.GetChildByName(_currentLayout, "DraggableItems");
            var dropZonesParent = Dlcs.Extensions.GetChildByName(_currentLayout, "DropZones");

            if (draggableItemsParent)
            {
                foreach (Transform child in draggableItemsParent.transform)
                {
                    var item = child.GetComponent<DraggableItem>();
                    if (item)
                    {
                        _draggableItems.Add(item);
                    }
                }
            }

            if (dropZonesParent)
            {
                foreach (Transform child in dropZonesParent.transform)
                {
                    var zone = child.GetComponent<DropZone>();
                    if (zone)
                    {
                        _dropZones.Add(zone);
                    }
                }
            }
        }
        
        private void SetTexts()
        {
            
            _questionText.text = _currentQuestion.QuestionText;
            _feedbackText.text = _currentQuestion.CorrectFeedback;
            

            foreach(var item in _currentQuestion.DraggableItems)
            {
                
                foreach (var draggableItem in _draggableItems)
                { 
                    if (draggableItem.name == item.ComponentName) 
                    { 
                        draggableItem.SetItemText(item.Text, item.SpecialText);
                        break;
                    }
                }
            }

            foreach(var item in _currentQuestion.DropZones)
            {
                
                foreach (var dropZone in _dropZones)
                {
                    if (dropZone.name == item.ComponentName)
                    { 
                        dropZone.SetItemText(item.Text, item.SpecialText);
                        break;
                    }
                    
                }
            }
        }

        private void DisplayQuestion()
        {
            foreach(var dropZone in _dropZones)
            {
                GameEvents.ForceDisappearDropZoneImages(dropZone);
            }
            
            questionPanel.SetActive(true);
            feedbackPanel.SetActive(false);
            restartButton.SetActive(false);
            nextButton.SetActive(false);
            
            var animator = _currentLayout.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetTrigger("Display");
            _animator.SetTrigger("ShowQuestion");
          
        }

        private void OnItemDropped(DraggableItem item, DropZone dropZone)
        {
            foreach(var correctMatch in _currentQuestion.CorrectMatches)
            {
                if (item.name == correctMatch.DraggableComponentName &&
                    dropZone.name == correctMatch.DropZoneComponentName)
                {
                    _playerMatches.Add(correctMatch);
                    GameEvents.AppearDropZoneImage(dropZone, item);
                    
                    Debug.Log("Current matches: " + _playerMatches.Count + " out of " + _currentQuestion.CorrectMatches.Count);
                    
                    if(_playerMatches.Count == _currentQuestion.CorrectMatches.Count) DisplayCorrectFeedback();
                    
                    return;
                }
            }
            
            GameEvents.ForceItemReturn(item);
            DisplayIncorrectFeedback();
            
        }

        private void DisplayCorrectFeedback()
        {   
            _feedbackText.text = _currentQuestion.CorrectFeedback;
            feedbackPanel.GetComponent<Image>().sprite = correctSprite;
            _animator.SetTrigger("ShowCorrectFeedback");
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(true);
            nextButton.SetActive(true);
        }

        private void DisplayIncorrectFeedback()
        {
            
            _feedbackText.text = _currentQuestion.IncorrectFeedback;
            feedbackPanel.GetComponent<Image>().sprite = incorrectSprite;
            _animator.SetTrigger("ShowIncorrectFeedback");
            questionPanel.SetActive(false);
            feedbackPanel.SetActive(true);
            restartButton.SetActive(true);
        }
        
        private GameObject FindCurrentQuestionLayout()
        {
            var layoutNumber = "layout" + _currentQuestion.LevelNumber + "." + _currentQuestion.QuestionNumber;
            
            foreach (var layout in layouts)
            {
                if (layout.name == layoutNumber)
                {
                    layout.SetActive(true);
                    return layout;
                }
                
                else layout.SetActive(false);
            }

            return null;
        }
        
        private void OnNextButtonClicked()
        {
            _playerMatches = new List<CorrectMatch>();
            foreach(var draggableItem in _draggableItems)
            {
                GameEvents.ForceItemReturn(draggableItem);
            }
            GameEvents.OnNextQuestion?.Invoke();
        }   
        
        private void OnRestartButtonClicked()
        {
            _playerMatches = new List<CorrectMatch>();
            foreach(var draggableItem in _draggableItems)
            {
                GameEvents.ForceItemReturn(draggableItem);
            }
            _currentLayout.SetActive(false);
            GameEvents.OnRestartLevel?.Invoke();
        }

        private void OnQuestionButtonClicked()
        {
            _animator.SetTrigger("HideQuestion");
        }

        private void EnableButtons()
        {
            _nextButton.interactable = true;
            _restartButton.interactable = true;
        }

        private void DisableButtons()
        {
            _nextButton.interactable = false;
            _restartButton.interactable = false;
        }

        private void DisableQuestionButton()
        {
            _questionContinueButton.interactable = false;
        }

        private void EnableQuestionButton()
        {
            _questionContinueButton.interactable = true;
        }

        private void DisableQuestion()
        {
            questionPanel.SetActive(false);
        }
    }
}