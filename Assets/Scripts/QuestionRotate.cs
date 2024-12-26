using UnityEngine;

namespace Hunter
{
    public class QuestionRotate : MonoBehaviour
    {
        public Animation ani;

        public void Show()
        {
            ani.Play("ShowQuestion");
        }
        
        public void Hide()
        {
            ani.Play("HideQuestion");
        }

        private void LateUpdate()
        {
            if(GameController.instance != null && transform.localScale != Vector3.zero)
            {
                transform.LookAt(GameController.instance.cam.transform);
            }
        }
    }
}
