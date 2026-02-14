using System;
using Dlcs;
using Global;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    public GameObject buttonPanel;
    
    public GameManager.GameLanguage defaultLanguage = GameManager.GameLanguage.Catalan;

    private static GameManager.GameLanguage _currentGameLanguage;
    
    private Button _englishButton;
    private Button _spanishButton;
    private Button _catalanButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _currentGameLanguage = defaultLanguage;
    }

    private void Start()
    {
        
        _englishButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Eng").GetComponent<Button>();
        _spanishButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Esp").GetComponent<Button>();
        _catalanButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Cat").GetComponent<Button>();
        
        _englishButton.onClick.AddListener(_changeToEnglish);
        _spanishButton.onClick.AddListener(ChangeToSpanish);
        _catalanButton.onClick.AddListener(ChangeToCatalan);
        
        _catalanButton.interactable = false;
    }
    
    private void _changeToEnglish()
    {
        ChangeToLanguage(GameManager.GameLanguage.English);
    }
    
    private void ChangeToSpanish()
    {
        ChangeToLanguage(GameManager.GameLanguage.Spanish);
    }
    
    private void ChangeToCatalan()
    {
        ChangeToLanguage(GameManager.GameLanguage.Catalan);
    }
    
    private void ChangeToLanguage(GameManager.GameLanguage language)
    {
        if (language == _currentGameLanguage) return;
        
        switch (_currentGameLanguage)
        {
           case GameManager.GameLanguage.English:
               _englishButton.interactable = true;
               break;
           case GameManager.GameLanguage.Spanish:
               _spanishButton.interactable = true;
               break;
           case GameManager.GameLanguage.Catalan:
               _catalanButton.interactable = true;
               break;
        }
        
        _currentGameLanguage = language;
        
        switch (_currentGameLanguage)
        {
            case GameManager.GameLanguage.English:
                _englishButton.interactable = false;
                break;
            case GameManager.GameLanguage.Spanish:
                _spanishButton.interactable = false;
                break;
            case GameManager.GameLanguage.Catalan:
                _catalanButton.interactable = false;
                break;
        }
        
        GameEvents.LanguageChanged?.Invoke(language);
    }

    public static GameManager.GameLanguage CurrentLanguage()
    {
        return _currentGameLanguage;
    }
}
