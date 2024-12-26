using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public PlayerTouchMovement playerTouchMovement;
        public GameObject lookAt;
        public ParticleSystem blood;

        public void Awake()
        {
            instance = this;
        }

        public void FixedUpdate()
        {
            animator.SetFloat("Speed", playerTouchMovement.GetMovemntAmount().magnitude);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bot"))
            {
                //Debug.LogWarning(other.name);
                animator.SetTrigger("Hit");
                lookAt = other.gameObject;
                Invoke("ChangeLookAt", 0.5f);
                Bot bot = GameController.instance.GetBot(other.gameObject);
                DOVirtual.DelayedCall(0.35f, delegate
                {
                    bot.PlayBlood();
                });
            }
        }

        void ChangeLookAt()
        {
            lookAt = gameObject;
        }

        public void PlayBlood()
        {
            blood.Play();
        }
    }
}
