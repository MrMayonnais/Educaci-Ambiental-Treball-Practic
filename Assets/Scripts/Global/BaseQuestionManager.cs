using UnityEngine;

public class BaseQuestionManager : MonoBehaviour
{
    public virtual void LoadQuestion(BaseQuestion question)
    {
        Debug.Log("Base LoadQuestion called");
    }
}