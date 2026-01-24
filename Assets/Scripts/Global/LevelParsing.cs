using System.Collections.Generic;
using System.Linq;
using Global.Types;
using NUnit.Framework;
using UnityEngine;

public class LevelParsing
{
    /*public static LevelData ParseLevel1(string fileContent)
    {
        LevelData levelData = new LevelData();
        levelData.questions = new List<BaseQuestion>();
        
        string[] lines = fileContent.Split('\n');
        
        // Primera línea es el nombre del nivel (entre comillas)
        levelData.levelName = lines[0].Trim('"', '\r');
        
        int i = 1;
        while (i < lines.Length)
        {
            string line = lines[i].Trim();
            
            // Detectar inicio de pregunta (nNombrePregunta)
            if (line.StartsWith("n"))
            {
                MultiChoiceQuestion question = new MultiChoiceQuestion();
                question.type = QuestionType.MultipleChoice;
                
                // Extraer número y nombre de pregunta
                string questionName = line.Substring(1);
                // Extraer número si existe al inicio
                int numberEnd = 0;
                while (numberEnd < questionName.Length && char.IsDigit(questionName[numberEnd]))
                    numberEnd++;
                
                if (numberEnd > 0)
                {
                    question.questionNumber = int.Parse(questionName.Substring(0, numberEnd));
                    question.questionText = questionName.Substring(numberEnd);
                }
                else
                {
                    question.questionText = questionName;
                }
                
                i++;
                
                // Leer opciones (a, b, c, d)
                if (i < lines.Length && lines[i].Trim().StartsWith("a"))
                    question.OptionA = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("b"))
                    question.OptionB = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("c"))
                    question.OptionC = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("d"))
                    question.OptionD = lines[i++].Trim().Substring(1);
                
                // Leer respuesta correcta [letra]
                if (i < lines.Length && lines[i].Trim().StartsWith("["))
                {
                    string answerLine = lines[i++].Trim();
                    question.Correct = answerLine.Trim('[', ']', '\r')[0];
                }
                
                // Leer feedback correcto (+texto)
                if (i < lines.Length && lines[i].Trim().StartsWith("+"))
                    question.CorrectFeedback = lines[i++].Trim().Substring(1);
                
                // Leer feedback incorrecto (-texto)
                if (i < lines.Length && lines[i].Trim().StartsWith("-"))
                    question.IncorrectFeeback = lines[i++].Trim().Substring(1);
                
                levelData.questions.Add(question);
            }
            else
            {
                i++;
            }
        }
        
        return levelData;
    }
    
    public static LevelData ParseLevel2(string fileContent)
    {
        LevelData levelData = new LevelData();
        levelData.questions = new List<BaseQuestion>();
        
        levelData.questions.Add(new BaseQuestion(QuestionType.DragAndDrop));
        levelData.questions.Add(new BaseQuestion(QuestionType.DragAndDrop));
        levelData.questions.Add(new BaseQuestion(QuestionType.DragAndDrop));
        levelData.questions.Add(new BaseQuestion(QuestionType.DragAndDrop));

        return levelData;
    }
    
    public static LevelData ParseLevel3(string fileContent)
    {
        // TODO: Implementar parser para nivel 3 cuando se defina el formato
        LevelData levelData = new LevelData();
        levelData.levelName = "Nivel 3";
        levelData.questions = new List<BaseQuestion>();
        Debug.LogWarning("Formato de Nivel 3 aún no definido");
        return levelData;
    }*/


    public GameData ParseAllQuestions(string fileContent)
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
    
    public DragAndDropQuestion ParseDragNDropQuestion(string[] lines, int currentIndex)
    {
        DragAndDropQuestion question = new DragAndDropQuestion();
        
        // Implementar parsing específico para DragAndDropQuestion aquí
        
        return question;
    }
    
    public MultiChoiceQuestion ParseMultiChoiceQuestion(string[] lines, int currentIndex)
    {
        MultiChoiceQuestion question = new MultiChoiceQuestion();

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

    private BaseQuestion ParseQuestionBlock(string block)
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
                    question = ParseMultiChoiceQuestion(lines, i);
                }
                else if (typePart == "D")
                {
                    question = ParseDragNDropQuestion(lines, i);
                }
            }
        }

        return question;
    }
    
    private GameData SortQuestionsIntoLevels(List<BaseQuestion> questions)
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
}