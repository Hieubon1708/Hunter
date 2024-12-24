using UnityEngine;

namespace Hunter
{
    public class ListeningDistance : MonoBehaviour
    {
        public Vector3 allyPosition;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bot"))
            {
                allyPosition = other.transform.parent.position;
                Debug.Log("Enter " + other.transform.parent.name + " position " + allyPosition);
            }
        }
    }
}
