using System.Collections.Generic;
using Global;
using PrimeTween;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{    
    public Color slide1Color;
    public Color slide2Color;
    public Color slide3Color;
    public Color slideEndColor;

    public Color questions1Color;
    public Color questions2Color;
    public Color questions3Color;

    private List<Color> _slideColors;
    private List<Color> _questionColors;

    private Color _currentColor;
    private int _currentSlideColorIndex = -1;
    private int _currentQuestionColorIndex;

    private Color _currentSlideColor;
    private Color _currentQuestionColor;

    private Tween _colorTween;
    
    private SpriteRenderer _backgroundRenderer;
    
    private void Start()
    {
        _slideColors = new List<Color>();
        _slideColors.Add(slide1Color);
        _slideColors.Add(slide2Color);
        _slideColors.Add(slide3Color);
        _slideColors.Add(slideEndColor);
        
        _questionColors = new List<Color>();
        _questionColors.Add(questions1Color);
        _questionColors.Add(questions2Color);
        _questionColors.Add(questions3Color);
        
        _backgroundRenderer = GetComponent<SpriteRenderer>();
    }
    
    
    
    private void OnEnable()
    {
        GameEvents.ChangeSlideBackground += ChangeSlideBackground;
        GameEvents.ChangeQuestionBackground += ChangeQuestionBackground;
    }
    
    private void OnDisable()
    {
        GameEvents.ChangeSlideBackground -= ChangeSlideBackground;
        GameEvents.ChangeQuestionBackground -= ChangeQuestionBackground;
    }
    
    private void ChangeSlideBackground()
    {
        if(_colorTween.isAlive) _colorTween.Stop();
        
        _currentSlideColorIndex++;
        var previousColor = _currentSlideColor;
        _currentColor = _slideColors[_currentSlideColorIndex];

        _colorTween = Tween.Color(_backgroundRenderer, previousColor, _currentColor, 1f);

    }
    
    private void ChangeQuestionBackground()
    {
        if(_colorTween.isAlive) _colorTween.Stop();
        
        _currentQuestionColorIndex++;
        var previousColor = _currentSlideColor;
        _currentColor = _questionColors[_currentQuestionColorIndex];

        _colorTween = Tween.Color(_backgroundRenderer, previousColor, _currentColor, 1f);
    }
}
