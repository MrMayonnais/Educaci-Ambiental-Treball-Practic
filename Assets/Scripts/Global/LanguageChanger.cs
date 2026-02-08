using Dlcs;
using Global;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    public GameObject buttonPanel;
    
    private Button _englishButton;
    private Button _spanishButton;
    private Button _catalanButton;
    
    void Start()
    {
        _englishButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Eng").GetComponent<Button>();
        _spanishButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Esp").GetComponent<Button>();
        _catalanButton = Dlcs.Extensions.GetChildByName(buttonPanel, "Cat").GetComponent<Button>();
        
        _englishButton.onClick.AddListener(ChangeToEnglish);
        _spanishButton.onClick.AddListener(ChangeToSpanish);
        _catalanButton.onClick.AddListener(ChangeToCatalan);
    }

    private void ChangeToEnglish()
    {
        GameEvents.LanguageChanged?.Invoke(GameManager.GameLanguage.English);
    }
    
    private void ChangeToSpanish()
    {
        GameEvents.LanguageChanged?.Invoke(GameManager.GameLanguage.Spanish);
    }
    
    private void ChangeToCatalan()
    {
        GameEvents.LanguageChanged?.Invoke(GameManager.GameLanguage.Catalan);
    }
}
