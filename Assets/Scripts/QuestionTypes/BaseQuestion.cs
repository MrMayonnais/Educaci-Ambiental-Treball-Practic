public class BaseQuestion
{
    public int questionNumber;
    public string questionText;
    public QuestionType type;
    
    public BaseQuestion(QuestionType questionType)
    {
        type = questionType;
    }
    
    public BaseQuestion(){}
}