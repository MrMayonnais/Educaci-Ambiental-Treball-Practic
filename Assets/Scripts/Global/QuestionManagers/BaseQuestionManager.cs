using UnityEngine;

namespace Global.QuestionManagers
{
    public class BaseQuestionManager : MonoBehaviour
    {
        public virtual void LoadQuestion(BaseQuestion question)
        {
            Debug.Log("Base LoadQuestion called");
        }
    }
}