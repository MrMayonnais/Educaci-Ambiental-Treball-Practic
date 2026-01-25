using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiChoiceQuestion : BaseQuestion
{
    public string OptionA { get; set; }
    public string OptionB { get; set; }
    public string OptionC  { get; set; }
    public string OptionD  { get; set; }
    public char CorrectAnswer { get; set; }


    public MultiChoiceQuestion(BaseQuestion question)
    {
        this.LevelNumber = question.LevelNumber;
        this.QuestionNumber = question.QuestionNumber;
        this.QuestionText = question.QuestionText;
        this.Type = question.Type;
        this.CorrectFeedback = question.CorrectFeedback;
        this.IncorrectFeedback = question.IncorrectFeedback;
    }
    
    public MultiChoiceQuestion(){}
}