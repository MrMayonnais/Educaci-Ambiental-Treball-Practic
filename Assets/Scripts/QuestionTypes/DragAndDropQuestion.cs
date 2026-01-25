using System;
using System.Collections.Generic;
using System.Linq;
using Global.Types;

public class DragAndDropQuestion : BaseQuestion
{
    public List<String> DraggableItems;
    public List<String> DraggableItemsText;
    public List<String> DraggableItemsSpecialText;
    
    public List<String> DropZones;
    public List<String> DropZonesText;
    public List<String> DropZonesSpecialText;
    
    public List<CorrectMatch> CorrectMatches;

    public DragAndDropQuestion()
    {
        base.Type = QuestionType.DragAndDrop;
        DraggableItems = new List<String>();
        DraggableItemsText = new List<String>();
        DraggableItemsSpecialText = new List<String>();
        
        DropZones = new List<String>();
        DropZonesText = new List<String>();
        DropZonesSpecialText = new List<String>();
        
        CorrectMatches = new List<CorrectMatch>();
    }
    
    public DragAndDropQuestion(int questionNumber, int levelNumber, string questionText, string correctFeedback, string incorrectFeedback,
        List<String> draggableItems, List<String> draggableItemsText, List<String> draggableItemsSpecialText,
        List<String> dropZones, List<String> dropZonesText, List<String> dropZonesSpecialText,
        List<CorrectMatch> correctMatches) : base(questionNumber, levelNumber, questionText, QuestionType.DragAndDrop, correctFeedback, incorrectFeedback)
    {
        DraggableItems = draggableItems;
        DraggableItemsText = draggableItemsText;
        DraggableItemsSpecialText = draggableItemsSpecialText;
        
        DropZones = dropZones;
        DropZonesText = dropZonesText;
        DropZonesSpecialText = dropZonesSpecialText;
        
        CorrectMatches = correctMatches;
    }

    public DragAndDropQuestion(BaseQuestion baseQuestion)
    {
        this.QuestionNumber = baseQuestion.QuestionNumber;
        this.LevelNumber = baseQuestion.LevelNumber;
        this.QuestionText = baseQuestion.QuestionText;
        this.Type = QuestionType.DragAndDrop;
        this.CorrectFeedback = baseQuestion.CorrectFeedback;
        this.IncorrectFeedback = baseQuestion.IncorrectFeedback;
    }
    
    
}