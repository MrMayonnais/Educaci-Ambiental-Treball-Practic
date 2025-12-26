using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Global;
using Unity.VisualScripting;

/*
public class DragQuestionManager : BaseQuestionManager
{
    [Header("UI References")]
    public TMP_Text questionText;
    public GameObject feedbackPanel;
    public TMP_Text feedbackText;
    public Button nextButton;
    public Button restartButton;

    [Header("Layouts")] 
    public GameObject layout4X4;
    public GameObject layout7X7;
    public GameObject layout9X9;
    
    [Header("Feedback Sprites")]
    public Sprite correctSprite;
    public Sprite incorrectSprite;
    
    private DragAndDropQuestion _currentQuestion;
    private bool _answered = false;
    private Dictionary<int, int> playerMatches = new Dictionary<int, int>();
    private GameObject _currentLayout;
    private List<DraggableItem> _draggableItems = new List<DraggableItem>();
    private List<DropZone> _dropZones = new List<DropZone>();

    void OnEnable()
    {
        LevelManager.GetInstance().OnQuestionLoaded += HandleQuestionLoaded;
    }

    void OnDisable()
    {
        LevelManager.GetInstance().OnQuestionLoaded -= HandleQuestionLoaded;
    }

    void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        
        nextButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        feedbackPanel.SetActive(false);
        
        layout4X4.SetActive(false);
        layout7X7.SetActive(false);
        layout9X9.SetActive(false);
    }
    
    void HandleQuestionLoaded(BaseQuestion question)
    {
        if (question is DragAndDropQuestion dragAndDropQuestion)
        {
            _currentQuestion = dragAndDropQuestion;
            DisplayQuestion();
        }
    }

    void DisplayQuestion()
    {
        _answered = false;
        playerMatches.Clear();
        _draggableItems.Clear();
        _dropZones.Clear();
        
        questionText.text = _currentQuestion.questionText;
        
        layout4X4.SetActive(false);
        layout7X7.SetActive(false);
        layout9X9.SetActive(false);
        
        _currentLayout = GetlayoutForQuestionType(_currentQuestion.type);
        _currentLayout.SetActive(true);
        
        Transform receptorsContainer = _currentLayout.transform.Find("DropZones");
        Transform itemsContainer = _currentLayout.transform.Find("DraggableItems");
        

        for (int i = 0; i < _currentQuestion.DraggableItems.Count; i++)
        {
            DraggableItem item = itemsContainer.GetChild(i).GetComponent<DraggableItem>();
            item.gameObject.SetActive(true);
            item.itemIndex = i;
            item.manager = this;
            item.ResetItem();
            
            Image itemImage = item.GetComponent<Image>();
            if (itemImage)
            {
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + _currentQuestion.DraggableItems[i]);
                if (sprite)
                {
                    itemImage.sprite = sprite;
                }
                else
                {
                    itemImage.sprite = null;
                    itemImage.color = Color.clear;
                }
            }
            
            TextMeshProUGUI itemText = item.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText)
            {
                itemText.text = _currentQuestion.DraggableTexts[i];
                Debug.Log("Setting text for draggable item " + i + ": " + _currentQuestion.DraggableTexts[i]);
            }
            _draggableItems.Add(item);
            Debug.Log("Item added :"+item.name);
        }
        
        for (int i = 0; i < _currentQuestion.Receptors.Count; i++)
        {
            DropZone zone = receptorsContainer.GetChild(i).GetComponent<DropZone>();
            zone.gameObject.SetActive(true);
            zone.zoneIndex = i;
            zone.manager = this;
            zone.ClearItem();
            
            Image zoneImage = zone.GetComponent<Image>();
            if (zoneImage)
            {
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + _currentQuestion.Receptors[i]);
                if (sprite)
                {
                    zoneImage.sprite = sprite;
                }
                else
                {
                    zoneImage.sprite = null;
                    zoneImage.color = Color.clear;
                }
            }
            
            TextMeshProUGUI zoneText = zone.GetComponentInChildren<TextMeshProUGUI>();
            if (zoneText)
            {
                zoneText.text = _currentQuestion.ReceptorTexts[i];
            }
            
            _dropZones.Add(zone);
        }

        feedbackPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    GameObject GetlayoutForQuestionType(QuestionType type)
    {
        switch (type)
        {
            case QuestionType.DragAndDrop4X4:
                return layout4X4;
            case QuestionType.DragAndDrop7X7:
                return layout7X7;
            case QuestionType.DragAndDrop9X9:
                return layout9X9;
            default:
                return layout4X4;
                
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void OnItemDropped(int itemIndex, int zoneIndex)
    {
        // Verificar si el match es correcto
        bool isCorrect = _currentQuestion.CorrectMatches.ContainsKey(itemIndex) && 
                         _currentQuestion.CorrectMatches[itemIndex] == zoneIndex;

        if (!isCorrect)
        {
            // Match incorrecto - mostrar feedback y permitir reintentar
            _answered = true;
            feedbackText.text = _currentQuestion.IncorrectFeedback;
            feedbackPanel.GetComponent<Image>().sprite = incorrectSprite;
            feedbackPanel.SetActive(true);
        
            // Deshabilitar interacción con todos los items
            foreach (var item in _draggableItems)
            {
                item.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        
            restartButton.gameObject.SetActive(true);
            return;
        }

        // Match correcto - registrar
        playerMatches[itemIndex] = zoneIndex;

        // Verificar si todos los items correctos han sido colocados
        if (playerMatches.Count == _currentQuestion.CorrectMatches.Count)
        {
            CheckAllAnswers();
        }
    }
    
    private void CheckAllAnswers()
    {
        if (_answered) return;

        _answered = true;

        feedbackText.text = _currentQuestion.CorrectFeedback;
        feedbackPanel.GetComponent<Image>().sprite = correctSprite;
        feedbackPanel.SetActive(true);

        foreach (var item in _draggableItems)
        {
            item.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        nextButton.gameObject.SetActive(true);
    }

    private void CheckAnswers()
    {
        if (_answered) return;

        _answered = true;
        bool allCorrect = true;

        foreach (var match in playerMatches)
        {
            if (!_currentQuestion.CorrectMatches.ContainsKey(match.Key) ||
                _currentQuestion.CorrectMatches[match.Key] != match.Value)
            {
                allCorrect = false;
                break;
            }
        }

        feedbackText.text = allCorrect ? 
            _currentQuestion.CorrectFeedback : 
            _currentQuestion.IncorrectFeedback;
        
        feedbackPanel.GetComponent<Image>().sprite = allCorrect ?
            correctSprite :
            incorrectSprite;
        
        feedbackPanel.SetActive(true);

        foreach (var item in _draggableItems)
        {
            item.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
        if (allCorrect)
            nextButton.gameObject.SetActive(true);
        else
            restartButton.gameObject.SetActive(true);
    }
    
    void OnNextButtonClicked()
    {
        LevelManager.GetInstance().NextQuestion();
    }

    void OnRestartButtonClicked()
    {
        Debug.Log("Restarting level...");
        LevelManager.GetInstance().RestartLevel();
    }
}
*/
