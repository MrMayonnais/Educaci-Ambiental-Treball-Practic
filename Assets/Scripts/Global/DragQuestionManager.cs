using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Global;
using Unity.VisualScripting;


public class DragQuestionManager : BaseQuestionManager
{
    [Header("UI References")]
    public GameObject questionPanel;
    public GameObject feedbackPanel;
    public GameObject nextButton;
    public GameObject restartButton;

    [Header("Layouts")] 
    public GameObject layout4X4;
    public GameObject layout7X7;
    public GameObject layout9X9;
    
    [Header("Feedback Sprites")]
    public Sprite correctSprite;
    public Sprite incorrectSprite;
    
    private DragAndDropQuestion _currentQuestion;
    private Dictionary<int, int> _playerMatches = new Dictionary<int, int>();
    private GameObject _currentLayout;
    private List<DraggableItem> _draggableItems = new List<DraggableItem>();
    private List<DropZone> _dropZones = new List<DropZone>();
    
    private Button _nextButton;
    private Button _restartButton;
    private TextMeshProUGUI _questionText;
    private Button _questionButton;
    private TextMeshProUGUI _feedbackText;
    

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    private void Awake()
    {
        _nextButton = nextButton.GetComponent<Button>();
        _restartButton = restartButton.GetComponent<Button>();
        
        _questionText = questionPanel.GetComponentInChildren<TextMeshProUGUI>();
        _questionButton = _questionText.GetComponentInChildren<Button>();
        
        _feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _nextButton.onClick.AddListener(OnNextButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        
        nextButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        feedbackPanel.SetActive(false);
        questionPanel.SetActive(false);
        
        layout4X4.SetActive(false);
        layout7X7.SetActive(false);
        layout9X9.SetActive(false);
    }

    public override void LoadQuestion(BaseQuestion question)
    {
        if (question is DragAndDropQuestion dragQuestion)
        {
            _currentQuestion = dragQuestion;
            DisplayQuestion();
        }
        else
        {
            Debug.LogError("Invalid question type passed to DragQuestionManager.");
        }
    }
    

    private void DisplayQuestion()
    {
        _playerMatches.Clear();
        _draggableItems.Clear();
        _dropZones.Clear();
        
        _questionText.text = _currentQuestion.questionText;
        
        layout4X4.SetActive(false);
        layout7X7.SetActive(false);
        layout9X9.SetActive(false);
        
        _currentLayout = GetLayoutForQuestionType(_currentQuestion.type);
        _currentLayout.SetActive(true);
        questionPanel.SetActive(true);
        _questionButton.gameObject.SetActive(true);
        
        var receptorsContainer = _currentLayout.transform.Find("DropZones");
        var itemsContainer = _currentLayout.transform.Find("DraggableItems");
        

        for (var i = 0; i < _currentQuestion.DraggableItems.Count; i++)
        {
            var item = itemsContainer.GetChild(i).GetComponent<DraggableItem>();
            item.gameObject.SetActive(true);
            item.itemIndex = i;
            item.manager = this;
            item.ResetItem();
            
            var itemImage = item.GetComponent<Image>();
            if (itemImage)
            {
                var sprite = Resources.Load<Sprite>("Sprites/" + _currentQuestion.DraggableItems[i]);
                if (sprite)
                {
                    itemImage.sprite = sprite;
                }
                else if(_currentQuestion.DraggableItems[i] == "")
                {
                    itemImage.sprite = null;
                    itemImage.color = Color.clear;  
                }
            }
            
            var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText)
            {
                itemText.text = _currentQuestion.DraggableTexts[i];
                Debug.Log("Setting text for draggable item " + i + ": " + _currentQuestion.DraggableTexts[i]);
            }
            
            
            var itemSpecialText = item.transform.Find("SpecialText")?.GetComponent<TextMeshProUGUI>();
            if (itemSpecialText)
            {
                itemSpecialText.text = _currentQuestion.DraggableSpecialTexts[i];
                Debug.Log("Setting special text for draggable item: " + i + " => " + _currentQuestion.DraggableSpecialTexts[i]);
            }
            _draggableItems.Add(item);
            Debug.Log("Item added :"+item.name);
        }
        
        for (var i = 0; i < _currentQuestion.Receptors.Count; i++)
        {
            var dropZone = receptorsContainer.GetChild(i).GetComponent<DropZone>();
            dropZone.gameObject.SetActive(true);
            dropZone.zoneIndex = i;
            dropZone.manager = this;
            dropZone.ClearItem();
            
            var zoneImage = dropZone.GetComponent<Image>();
            if (zoneImage)
            {
                var sprite = Resources.Load<Sprite>("Sprites/" + _currentQuestion.Receptors[i]);
                if (sprite)
                {
                    zoneImage.sprite = sprite;
                }
                else if(_currentQuestion.Receptors[i] == "")
                {
                    zoneImage.sprite = null;
                    zoneImage.color = Color.clear;
                }
            }
            
            var zoneText = dropZone.GetComponentInChildren<TextMeshProUGUI>();
            if (zoneText)
            {
                zoneText.text = _currentQuestion.ReceptorTexts[i];
            }
            
            var zoneSpecialText = dropZone.transform.Find("SpecialText")?.GetComponent<TextMeshProUGUI>();
            if (zoneSpecialText)
            {
                zoneSpecialText.text = _currentQuestion.ReceptorSpecialTexts[i];
                Debug.Log("Setting special text for dropzone: " + i + " => " + _currentQuestion.DraggableSpecialTexts[i]);
            }
            
            _dropZones.Add(dropZone);
        }

        feedbackPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    private GameObject GetLayoutForQuestionType(QuestionType type)
    {
        return type switch
        {
            QuestionType.DragAndDrop4X4 => layout4X4,
            QuestionType.DragAndDrop7X7 => layout7X7,
            QuestionType.DragAndDrop9X9 => layout9X9,
            _ => layout4X4
        };
    }
    
    public void OnItemDropped(int itemIndex, int zoneIndex)
    {
        _playerMatches[itemIndex] = zoneIndex;
        var isCorrect = CheckAnswer(itemIndex, zoneIndex);

        if (!isCorrect) DisplayIncorrectFeedback();
        
        else if (_playerMatches.Count == _currentQuestion.CorrectMatches.Count)
        {
            DisplayCorrectFeedback();
        }
    }
    
    private void DisplayCorrectFeedback()
    {
        _feedbackText.text = _currentQuestion.CorrectFeedback;
        feedbackPanel.GetComponent<Image>().sprite = correctSprite;
        feedbackPanel.SetActive(true);

        foreach (var item in _draggableItems)
        {
            item.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        nextButton.gameObject.SetActive(true);
    }
    
    private void DisplayIncorrectFeedback()
    {
        _feedbackText.text = _currentQuestion.IncorrectFeedback;
        feedbackPanel.GetComponent<Image>().sprite = incorrectSprite;
        feedbackPanel.SetActive(true);
    }
    
    private bool CheckAnswer(int itemIndex, int zoneIndex)
    {
        return _currentQuestion.CorrectMatches[itemIndex] == zoneIndex;
    }
    
    private void OnNextButtonClicked()
    {
        GameEvents.OnNextQuestion?.Invoke();
    }

    private void OnRestartButtonClicked()
    {
        Debug.Log("Restarting level...");
        GameEvents.OnRestartLevel?.Invoke();
    }
    
    private void RestartGame()
    {
        _playerMatches.Clear();
        DisplayQuestion();
    }

    private void EnableQuestionButton(bool enable)
    {
        _questionButton.interactable = enable;
    }
    
    private void EnableLevelButtons(bool enable)
    {
        _nextButton.interactable = enable;
        _restartButton.interactable = enable;
    }
}

