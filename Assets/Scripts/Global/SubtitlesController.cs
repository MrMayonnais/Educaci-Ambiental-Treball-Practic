using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dlcs;
using Global;
using TMPro;
using UnityEngine;

public class SubtitlesController : MonoBehaviour
{
    private IntroSubtitles _introSubtitles;

    public GameObject subtitlePanel;

    private TextMeshProUGUI _subtitleText;

    public TextAsset catSubtitles;
    public TextAsset englishSubtitles;
    public TextAsset spanishSubtitles;

    private GameManager.GameLanguage _currentLanguage = GameManager.GameLanguage.Catalan;

    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine _typingCoroutine;

    private void Awake()
    {
        _subtitleText = subtitlePanel.GetComponentInChildren<TextMeshProUGUI>();

        _introSubtitles = new IntroSubtitles()
        {
            CatSlides = ParseSubtitlesFromText(catSubtitles.text),
            CastSlides = ParseSubtitlesFromText(spanishSubtitles.text),
            EngSlides = ParseSubtitlesFromText(englishSubtitles.text)
        };
    }

    private void OnEnable()
    {
        GameEvents.LanguageChanged += ChangeLanguage;
    }

    private void OnDisable()
    {
        GameEvents.LanguageChanged -= ChangeLanguage;
        StopTyping();
    }
    
    public void PlaySubtitles(int slideIndex)
    {
        var textToDisplay = GetSubtitleForSlide(slideIndex);
        
        if (!string.IsNullOrEmpty(textToDisplay))
        {
            ShowSubtitle(textToDisplay);
        }
    }

    private void ShowSubtitle(string text)
    {
        StopTyping();
        _typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        _subtitleText.text = "";

        foreach (var c in text)
        {
            _subtitleText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void StopTyping()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
    }

    public void CompleteTyping(string fullText)
    {
        StopTyping();
        _subtitleText.text = fullText;
    }

    public void CancelTyping()
    {
        StopTyping();
        _subtitleText.text = "";
    }

    private string GetSubtitleForSlide(int slideIndex)
    {
        switch (_currentLanguage)
        {
            case GameManager.GameLanguage.Spanish:
                
                return _introSubtitles.CastSlides[slideIndex];
            case GameManager.GameLanguage.English:
                
                return _introSubtitles.EngSlides[slideIndex];
            case GameManager.GameLanguage.Catalan:
            default:
                
                return _introSubtitles.CatSlides[slideIndex];
        }
    }

    private List<string> ParseSubtitlesFromText(string text)
    {
        var blocks = new List<string>(text.Trim().Split("//"));
        

        return blocks
            .Where(block => !string.IsNullOrEmpty(block))
            .Select(block => block.Trim())
            .ToList();
    }
    
    private void ChangeLanguage(GameManager.GameLanguage language)
    {
        _currentLanguage = language;
    }
    
    public class IntroSubtitles
    {
        public List<string> CatSlides;
        public List<string> CastSlides;
        public List<string> EngSlides;

        public IntroSubtitles()
        {
            CatSlides = new List<string>();
            CastSlides = new List<string>();
            EngSlides = new List<string>();
        }
    }
}