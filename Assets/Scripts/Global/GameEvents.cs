using System;
using Global.Types;
using UnityEngine;

namespace Global
{
    public class GameEvents
    {
        public static Action OnRestartLevel;
        public static Action OnNextQuestion;
        public static Action<DropZone> AppearDropZoneImage;
        public static Action<DraggableItem> ForceItemReturn;
        public static Action<DropZone> ForceDisappearDropZoneImage;
        
        public static Action LanguageChanged;
        public static Action<BaseQuestion> LoadQuestion;

    }
}