using System.Collections.Generic;
using System.Linq;

public class DragAndDropQuestion : BaseQuestion
{
    public List<string> DraggableItems;
    public List<string> DraggableTexts;
    public List<string> DraggableSpecialTexts;
    public List<string> Receptors;
    public List<string> ReceptorTexts;
    public List<string> ReceptorSpecialTexts;
    public Dictionary<int, int> CorrectMatches;
    public string CorrectFeedback;
    public string IncorrectFeedback;
    
}