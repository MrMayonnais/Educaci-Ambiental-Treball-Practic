using System;
using Dlcs;
using Global.Types;
using UnityEngine;

namespace Global
{
    public class GameEvents
    {
        
        public static Action OnStartLevel;
        public static Action OnRestartLevel;
        public static Action OnNextQuestion;
        public static Action<DropZone, DraggableItem> AppearDropZoneImage;
        public static Action<DraggableItem> ForceItemReturn;
        public static Action<DropZone> ForceDisappearDropZoneImages;
        
        public static Action<GameManager.GameLanguage> LanguageChanged;
        public static Action<BaseQuestion> LoadQuestion;

    }
}