using UnityEngine;

namespace Hunter
{
    public class Weapon : MonoBehaviour
    {
        public float attackRange;
        public int damage;
        public Quaternion startRotation;
        public Vector3 startPosition;

        private void Awake()
        {
            startRotation = transform.localRotation;
            startPosition = transform.localPosition;
        }

        public void ResetWeapon()
        {
            transform.localRotation = startRotation;
            transform.localPosition = startPosition;
        }
    }
}
