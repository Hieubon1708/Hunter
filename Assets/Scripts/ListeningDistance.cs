using UnityEngine;

namespace Hunter
{
    public class ListeningDistance : MonoBehaviour
    {
        public Bot bot;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bot") && !bot.isFind)
            {
                bot.StartHear(other.transform.position);
                Debug.Log("Enter " + other.transform.parent.name + " position " + other.transform.position);
            }
        }
    }
}
