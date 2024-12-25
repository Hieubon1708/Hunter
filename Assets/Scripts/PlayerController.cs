using UnityEngine;

namespace Hunter
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public void Awake()
        {
            instance = this;
        }
    }
}
