using System;
using System.Collections.Generic;
using System.Linq;

public class DragAndDropQuestion : BaseQuestion
{
    public List<String> DraggableItems;
    public List<String> DraggableItemsText;
    public List<String> DraggableItemsSpecialText;
    
    public List<String> DropZones;
    public List<String> DropZonesText;
    public List<String> DropZonesSpecialText;
    
    public Dictionary<String, List<String>> CorrectMatches;

    public DragAndDropQuestion()
    {
        base.Type = QuestionType.DragAndDrop;
        DraggableItems = new List<String>();
        DraggableItemsText = new List<String>();
        DraggableItemsSpecialText = new List<String>();
        
        DropZones = new List<String>();
        DropZonesText = new List<String>();
        DropZonesSpecialText = new List<String>();
        
        CorrectMatches = new Dictionary<String, List<String>>();
    }
    
    public DragAndDropQuestion(int questionNumber, int levelNumber, string questionText, string correctFeedback, string incorrectFeedback,
        List<String> draggableItems, List<String> draggableItemsText, List<String> draggableItemsSpecialText,
        List<String> dropZones, List<String> dropZonesText, List<String> dropZonesSpecialText,
        Dictionary<String, List<String>> correctMatches) : base(questionNumber, levelNumber, questionText, QuestionType.DragAndDrop, correctFeedback, incorrectFeedback)
    {
        DraggableItems = draggableItems;
        DraggableItemsText = draggableItemsText;
        DraggableItemsSpecialText = draggableItemsSpecialText;
        
        DropZones = dropZones;
        DropZonesText = dropZonesText;
        DropZonesSpecialText = dropZonesSpecialText;
        
        CorrectMatches = correctMatches;
    }
    
    

}