using UnityEngine;

namespace Hunter
{
    public class QuestionRotate : MonoBehaviour
    {
        public Animation animation;

        public void Show()
        {
            animation.Play("ShowQuestion");
        }
        
        public void Hide()
        {
            animation.Play("HideQuestion");
        }

        private void FixedUpdate()
        {
            if(GameController.instance != null && transform.localScale != Vector3.zero)
            {
                transform.LookAt(GameController.instance.cam.transform);
            }
        }
    }
}
