using UnityEngine;

namespace Hunter
{
    public class ListeningDistance : MonoBehaviour
    {
        public SentryBot bot;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bot") && !bot.isFind)
            {
                bot.StartHear(other.gameObject);
                Debug.Log("Enter " + other.transform.parent.name + " position " + other.transform.position);
            }
        }
    }
}
