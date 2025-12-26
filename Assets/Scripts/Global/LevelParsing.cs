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

        string[] lines = fileContent.Split('\n');

        // Primera línea es el nombre del nivel
        levelData.levelName = lines[0].Trim('"', '\r');

        int i = 1;
        while (i < lines.Length)
        {
            string line = lines[i].Trim();

            // Detectar inicio de pregunta (nNombrePregunta)
            if (line.StartsWith("n"))
            {
                DragAndDropQuestion question = new DragAndDropQuestion();
                question.DraggableItems = new List<string>();
                question.DraggableTexts = new List<string>();
                question.Receptors = new List<string>();
                question.ReceptorTexts = new List<string>();
                question.CorrectMatches = new Dictionary<int, int>();

                // Extraer nombre de pregunta
                question.questionText = line.Substring(1);

                // Extraer número si existe
                int numberEnd = 0;
                while (numberEnd < question.questionText.Length && char.IsDigit(question.questionText[numberEnd]))
                    numberEnd++;

                if (numberEnd > 0)
                {
                    question.questionNumber = int.Parse(question.questionText.Substring(0, numberEnd));
                    question.questionText = question.questionText.Substring(numberEnd);
                }

                i++;
                int receptorIndex = 0;
                int draggableIndex = 0;

                // Leer pares de imágenes (a1/a2, b1/b2, c1/c2, d1/d2, etc.)
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i].Trim()))
                {
                    string pairLine = lines[i].Trim();

                    if (pairLine.StartsWith("+"))
                    {
                        question.CorrectFeedback = pairLine.Substring(1);
                        i++;
                    }
                    else if (pairLine.StartsWith("-"))
                    {
                        question.IncorrectFeedback = pairLine.Substring(1);
                        i++;
                        break; // Fin de la pregunta
                    }
                    else if (pairLine.Length > 2 && char.IsLetter(pairLine[0]))
                    {
                        char letter = pairLine[0];
                        char number = pairLine[1];

                        // Remover letra y número, obtener datos
                        string data = pairLine.Substring(2);
                        string[] parts = data.Split('_');

                        string imageName = parts.Length > 0 ? parts[0].Trim() : "";
                        string text = parts.Length > 1 ? parts[1].Trim('"', ' ') : "";

                        // Si nombre y texto están vacíos, es un objeto invisible
                        bool isEmpty = string.IsNullOrEmpty(imageName) && string.IsNullOrEmpty(text);

                        if (number == '1') // Receptor (DropZone)
                        {
                            question.Receptors.Add(imageName);
                            question.ReceptorTexts.Add(text);
                            receptorIndex++;
                            Debug.Log("added receptor " + receptorIndex + ": " + imageName + " / " + text);
                        }
                        else if (number == '2') // DraggableItem
                        {
                            question.DraggableItems.Add(imageName);
                            question.DraggableTexts.Add(text);
                            
                            // Solo crear match si no está vacío
                            if (!isEmpty)
                            {
                                // El match se crea entre el draggableIndex y el receptorIndex-1
                                // porque acabamos de leer el receptor correspondiente
                                question.CorrectMatches[draggableIndex] = receptorIndex - 1;
                            }
                            
                            draggableIndex++;
                            Debug.Log("added draggable " + draggableIndex + ": " + imageName + " / " + text);
                        }

                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }

                // Determinar el tipo según la cantidad de receptores
                switch (question.Receptors.Count)
                {
                    case 4:
                        question.type = QuestionType.DragAndDrop4X4;
                        break;
                    case 7:
                        question.type = QuestionType.DragAndDrop7X7;
                        break;
                    case 9:
                        question.type = QuestionType.DragAndDrop9X9;
                        break;
                    default:
                        Debug.LogWarning($"Cantidad de receptores no soportada: {question.Receptors.Count}");
                        question.type = QuestionType.DragAndDrop4X4;
                        break;
                }

                levelData.questions.Add(question);
            }
            else
            {
                i++;
            }
        }

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