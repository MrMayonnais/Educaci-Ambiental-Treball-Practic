using System;
using System.Collections.Generic;
using System.Linq;
using Global.Types;

public class DragAndDropQuestion : BaseQuestion
{
    public List<DraggableItemInfo> DraggableItems;
    public List<DropZoneInfo> DropZones;
    
    public List<CorrectMatch> CorrectMatches;

    public DragAndDropQuestion()
    {
        base.Type = QuestionType.DragAndDrop;
        
        DraggableItems = new List<DraggableItemInfo>();
        DropZones = new List<DropZoneInfo>();
        
        CorrectMatches = new List<CorrectMatch>();
    }

    public DragAndDropQuestion(BaseQuestion baseQuestion)
    {
        this.QuestionNumber = baseQuestion.QuestionNumber;
        this.LevelNumber = baseQuestion.LevelNumber;
        this.QuestionText = baseQuestion.QuestionText;
        this.Type = QuestionType.DragAndDrop;
        this.CorrectFeedback = baseQuestion.CorrectFeedback;
        this.IncorrectFeedback = baseQuestion.IncorrectFeedback;
        
        DraggableItems = new List<DraggableItemInfo>();
        DropZones = new List<DropZoneInfo>();
        CorrectMatches = new List<CorrectMatch>();
    }
    
    public int GetTotalMatches()
    {
        var count = 0;
        
        foreach (var match in CorrectMatches)
        {
            if(!string.IsNullOrEmpty(match.DraggableComponentName) && !string.IsNullOrEmpty(match.DropZoneComponentName))
                count++;
        }
        
        return count;
    }
    
    public struct DraggableItemInfo
    {
        public string ComponentName;
        public string Text;
        public string SpecialText;
    }
    
    public struct DropZoneInfo
    {
        public string ComponentName;
        public string Text;
        public string SpecialText;
    }
}