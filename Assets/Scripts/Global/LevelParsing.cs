using System.Collections.Generic;
using System.Linq;
using Global.Types;
using NUnit.Framework;
using UnityEngine;

public class LevelParsing
{
    public static GameData ParseAllQuestions(string fileContent)
    {
        var gameData = new GameData
        {
            Levels = new List<LevelData>()
        };
        
        var questionBlocks = fileContent.Split(new[] { "//" }, System.StringSplitOptions.RemoveEmptyEntries);

        var questions = new List<BaseQuestion>();
        
        foreach (var block in questionBlocks)
        {
            questions.Add(ParseQuestionBlock(block));
        }


        gameData = SortQuestionsIntoLevels(questions);
        
        
        return gameData;
    }
    
    private static DragAndDropQuestion ParseDragNDropQuestion(string[] lines, int currentIndex, BaseQuestion baseQuestion = null)
    {
        DragAndDropQuestion question = new DragAndDropQuestion(baseQuestion);
        
        for (int i = currentIndex; i < lines.Length; i++)
        {
            var line = lines[i];

            if (char.IsLetter(line.Substring(0, 1)[0]))
            {
                if (line.Substring(1, 1) == "<")
                {
                    question.CorrectMatches.Add(CreateCorrectMatchFromLines(lines[i], lines[i + 1]));
                    i++;
                }
                    
            }
            else if (line.StartsWith("+")) question.CorrectFeedback = line.Trim('+').Trim();
            else if (line.StartsWith("-")) question.IncorrectFeedback = line.Trim('-').Trim();
        }
        
        return question;
    }
    
    private static MultiChoiceQuestion ParseMultiChoiceQuestion(string[] lines, int currentIndex, BaseQuestion baseQuestion = null)
    {
        MultiChoiceQuestion question = new MultiChoiceQuestion(baseQuestion);

        for (int i = currentIndex; i < lines.Length; i++)
        {
            var line = lines[i];

            if (line.StartsWith("#")) question.QuestionText = line.Trim('#');
            else if (line.StartsWith("a")) question.OptionA = line.Trim('a').Trim();
            else if (line.StartsWith("b")) question.OptionB = line.Trim('b').Trim();
            else if (line.StartsWith("c")) question.OptionC = line.Trim('c').Trim();
            else if (line.StartsWith("d")) question.OptionD = line.Trim('d').Trim();
            else if (line.StartsWith("?")) question.CorrectAnswer = line.Trim('?').Trim()[0];
            else if (line.StartsWith("+")) question.CorrectFeedback = line.Trim('+').Trim();
            else if (line.StartsWith("-")) question.IncorrectFeedback = line.Trim('-').Trim();
        }
        
        return question;
    }

    private static BaseQuestion ParseQuestionBlock(string block)
    {
        BaseQuestion question = new BaseQuestion();

        var lines = block.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
        
            if (line.StartsWith("*"))
            {
                var parts = line.Trim('*').Split('.');
                question.LevelNumber = int.Parse(parts[0]);
                question.QuestionNumber = int.Parse(parts[1]);
            }

            if (line.StartsWith("!"))
            {
                var typePart = line.Trim('!').Trim();

                if (typePart == "M")
                {
                    question = ParseMultiChoiceQuestion(lines, i, question);
                }
                else if (typePart == "D")
                {
                    question = ParseDragNDropQuestion(lines, i, question);
                }
            }
        }

        return question;
    }
    
    private static GameData SortQuestionsIntoLevels(List<BaseQuestion> questions)
    {
        var gameData = new GameData
        {
            Levels = new List<LevelData>()
        };

        foreach (var question in questions)
        {
            if (gameData.Levels.Last() == null || gameData.Levels.Last().LevelNumber != question.LevelNumber)
            {
                gameData.Levels.Add(new LevelData()
                {
                    LevelNumber = question.LevelNumber,
                    Questions = new List<BaseQuestion>()
                });
            }
            
            gameData.Levels.Last().Questions.Add(question);
        }
        
        return gameData;
    }
    
    private static CorrectMatch CreateCorrectMatchFromLines(string draggableLine, string dropZoneLine)
    {
        var draggableParts = draggableLine.Substring(2).Split('%', '$');
        var draggableComponentName = draggableParts[0];
        var draggableText = draggableParts.Length > 1 ? draggableParts[1] : "";
        var draggableSpecialText = draggableParts.Length > 2 ? draggableParts[2] : "";
        
        var dropZoneParts = dropZoneLine.Substring(2).Split('%', '$');
        var dropZoneComponentName = dropZoneParts[0];
        var dropZoneText = dropZoneParts.Length > 1 ? dropZoneParts[1] : "";
        var dropZoneSpecialText = dropZoneParts.Length > 2 ? dropZoneParts[2] : "";
        
        return new CorrectMatch
        {
            DraggableComponentName = draggableComponentName,
            DraggableText = draggableText,
            DraggableSpecialText = draggableSpecialText,
            DropZoneComponentName = dropZoneComponentName,
            DropZoneText = dropZoneText,
            DropZoneSpecialText = dropZoneSpecialText
        };
    }
}