using System;
using Global;
using UnityEngine;

namespace Intro
{
    public interface ISlidesController
    {

        public static Action OnStartSlideShow;
        public static Action OnNextSlide;
        public static Action OnSlideShowFinished;

        void NextSlide();

        void SwapSlide();
        
        void LoadSlide(GameObject slide);
        
        void FinishSlideShow();

        void ChangeLanguage(GameManager.GameLanguage newLanguage);

        void AnimateSlideEntrance(GameObject slide);

        void AnimateSlideExit(GameObject slide);

        void AnimateEntrance();

        void AnimateExit();

        void GetCurrentSlides();
    }
}
