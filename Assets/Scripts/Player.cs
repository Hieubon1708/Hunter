using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class Player : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public GameObject lookAt;
        public ParticleSystem blood;
        public CapsuleCollider attackRangeCollider;
        public Transform hand;
        public LayerMask botLayer;
        public bool isKilling;

        public void Init(float attackRange)
        {
            attackRangeCollider.radius = attackRange;
        }      

        public void OnTriggerStay(Collider other)
        {
            if (!isKilling && other.CompareTag("Bot"))
            {
                RaycastHit hit;
                Vector3 from = transform.position;
                Vector3 to = other.transform.position;
                from.y += 0.1f;
                to.y += 0.1f;
                Vector3 direction = to - from;
                Physics.Raycast(from, direction, out hit, 10);
                if (hit.collider != null && hit.collider.tag == "Bot")
                {
                    isKilling = true;
                    //Debug.LogWarning(other.name);
                    animator.SetTrigger("Hit");
                    lookAt = other.gameObject;
                    Invoke("ChangeLookAt", 0.5f);
                    Bot bot = GameController.instance.GetBot(other.gameObject);
                    DOVirtual.DelayedCall(0.35f, delegate
                    {
                        bot.PlayBlood();
                        bot.SubtractHp(GameController.instance.weaponEquip.scWeapon.damage);
                        DOVirtual.DelayedCall(0.35f, delegate
                        {
                            isKilling = false;
                        });
                    });
                }
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

        public void SubtractHp(int hp)
        {
        }
    }
}
