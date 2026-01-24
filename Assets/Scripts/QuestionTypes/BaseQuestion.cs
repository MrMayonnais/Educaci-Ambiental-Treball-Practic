public class BaseQuestion
{
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public string CorrectFeedback { get; set; }
    public string IncorrectFeedback { get; set; }
    
    public int LevelNumber { get; set; }
    public int QuestionNumber { get; set; }
    
    public BaseQuestion(int questionNumber, int levelNumber, string questionText, QuestionType questionType, string correctFeedback, string incorrectFeedback)
    {
        QuestionNumber = questionNumber;
        LevelNumber = levelNumber;
        QuestionText = questionText;
        Type = questionType;
        CorrectFeedback = correctFeedback;
        IncorrectFeedback = incorrectFeedback;
    }
    
    public BaseQuestion(){}
}