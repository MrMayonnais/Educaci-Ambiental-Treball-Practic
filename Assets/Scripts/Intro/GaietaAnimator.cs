using Intro;
using UnityEngine;

public class GaietaAnimator : MonoBehaviour
{

    public Animator gaietaAnimator;
    
    private void OnEnable()
    {
        ISlidesController.OnStartSlideShow += StartSequence;
        ISlidesController.OnNextSlide += NextAnimation;
        ISlidesController.OnSlideShowFinished += EndSequence;
    }

    private void OnDisable()
    {
        ISlidesController.OnStartSlideShow -= StartSequence;
        ISlidesController.OnNextSlide -= NextAnimation;
        ISlidesController.OnSlideShowFinished -= EndSequence;
    }
    
    private void NextAnimation()
    {
        gaietaAnimator.SetTrigger("Next");
    }

    private void EndSequence()
    {
        gaietaAnimator.SetTrigger("End");
    }
    
    private void StartSequence()
    {
        gaietaAnimator.SetTrigger("Start");
    }
}
