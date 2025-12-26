using System.Collections.Generic;

public class DragAndDropQuestion : BaseQuestion
{
    public List<string> DraggableItems;
    public List<string> DraggableTexts;
    public List<string> Receptors;
    public List<string> ReceptorTexts;
    public Dictionary<int, int> CorrectMatches;
    public string CorrectFeedback;
    public string IncorrectFeedback;
}