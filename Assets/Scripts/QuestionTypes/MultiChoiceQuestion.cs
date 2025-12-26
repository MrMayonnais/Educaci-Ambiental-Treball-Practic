using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiChoiceQuestion : BaseQuestion
{
    public string optionA;
    public string optionB;
    public string optionC;
    public string optionD;
    public char correctAnswer; // 'a', 'b', 'c', 'd'
    public string correctFeedback;
    public string incorrectFeedback;
}