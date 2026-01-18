using System;
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

    }
}