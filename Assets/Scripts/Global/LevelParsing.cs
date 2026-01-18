using System.Collections.Generic;
using UnityEngine;

public class LevelParsing
{
    public static LevelData ParseLevel1(string fileContent)
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
                    question.optionA = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("b"))
                    question.optionB = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("c"))
                    question.optionC = lines[i++].Trim().Substring(1);
                if (i < lines.Length && lines[i].Trim().StartsWith("d"))
                    question.optionD = lines[i++].Trim().Substring(1);
                
                // Leer respuesta correcta [letra]
                if (i < lines.Length && lines[i].Trim().StartsWith("["))
                {
                    string answerLine = lines[i++].Trim();
                    question.correctAnswer = answerLine.Trim('[', ']', '\r')[0];
                }
                
                // Leer feedback correcto (+texto)
                if (i < lines.Length && lines[i].Trim().StartsWith("+"))
                    question.correctFeedback = lines[i++].Trim().Substring(1);
                
                // Leer feedback incorrecto (-texto)
                if (i < lines.Length && lines[i].Trim().StartsWith("-"))
                    question.incorrectFeedback = lines[i++].Trim().Substring(1);
                
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
    }
}