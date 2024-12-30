using UnityEngine;

namespace Hunter
{
    public class Bullet : MonoBehaviour
    {
        public BossBot bot;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = GameController.instance.GetPoppy(other.gameObject);
                if (player != null)
                {
                    player.Die(bot.transform);
                }
            }
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector3.forward * Time.fixedDeltaTime * bot.speed);
        }
    }
}
