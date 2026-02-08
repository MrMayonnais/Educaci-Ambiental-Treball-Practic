using System;
using System.Collections;
using System.Collections.Generic;
using Dlcs;
using Global;
using TMPro;
using UnityEngine;

public class SubtitlesController : MonoBehaviour
{
    private IntroSubtitles _introSubtitles;

    public GameObject subtitlePanel;

    private TextMeshProUGUI _subtitleText;

    private GameManager.GameLanguage _currentLanguage = GameManager.GameLanguage.Catalan;

    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine _typingCoroutine;

    private void Start()
    {
        _subtitleText = subtitlePanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        StopTyping();
    }
    
    public void PlaySubtitles(int slideIndex)
    {
        string textToDisplay = GetSubtitleForSlide(slideIndex);
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
                return _introSubtitles.CastSlides[slideIndex];
        }
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