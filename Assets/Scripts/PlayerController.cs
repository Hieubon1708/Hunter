using UnityEngine;

namespace Hunter
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public PlayerTouchMovement playerTouchMovement;

        public void Awake()
        {
            instance = this;
        }

        public float GetSpeed()
        {
            return playerTouchMovement.GetMovemntAmount().magnitude;
        }
    }
}
