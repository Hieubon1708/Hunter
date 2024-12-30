using UnityEngine;

namespace Hunter
{
    public class QuestionRotate : MonoBehaviour
    {
        public Bot bot;
        public Animation ani;

        public void Show()
        {
            if (!bot.col.enabled) return;
            ani.Play("ShowQuestion");
        }
        
        public void Hide()
        {
            if(transform.localScale == Vector3.zero) return;
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
