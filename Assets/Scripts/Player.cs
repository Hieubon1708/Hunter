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
        public Transform hips;
        public Rigidbody[] rbs;
        public CapsuleCollider col;
        public Weapon weapon;
        public AIUnit aIUnit;
        public float xExtraRadius;
        public float yExtraRadius;
        Tween delayKill;

        private void Start()
        {
            rbs = hips.GetComponentsInChildren<Rigidbody>();
            IsKinematic(true);
        }

        public void Init(Weapon weapon)
        {
            this.weapon = weapon;
            attackRangeCollider.radius = weapon.attackRange;
        }

        public void LoadWeapon(GameController.WeaponType weaponType)
        {
            GameController.instance.weaponEquip.Equip(this, weaponType);
        }

        public void OnTriggerStay(Collider other)
        {
            if (!col.enabled) return;
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
                    //Debug.LogError("isKilling");
                    isKilling = true;
                    lookAt = other.gameObject;

                    Bot bot = GameController.instance.GetBot(other.gameObject);
                    animator.SetTrigger("Hit");
                    Invoke("ChangeLookAt", 0.5f);
                    delayKill = DOVirtual.DelayedCall(0.35f, delegate
                    {
                        bot.SubtractHp(weapon.damage);
                        delayKill = DOVirtual.DelayedCall(0.35f, delegate
                        {
                            isKilling = false;
                        });
                    });
                }
            }
            if(GameController.instance.bots.Count == 0 && other.CompareTag("EndPoint") && !UIController.instance.gamePlay.layerCover.activeSelf)
            {
                StartCoroutine(PlayerController.instance.Win(other.transform.position));
                UIController.instance.Win();
            }
        }

        public void Die(Transform killer)
        {
            GameController.instance.RemovePoppy(this);
            delayKill.Kill();
            CancelInvoke("ChangeLookAt");
            UIController.instance.HitCancel();
            UIController.instance.ShakeCancel();
            col.enabled = false;
            animator.enabled = false;
            navMeshAgent.enabled = false;
            IsKinematic(false);
            Vector3 dir = PlayerController.instance.transform.position - killer.position;
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(new Vector3(dir.x, dir.y + 0.5f, dir.z) * 1.5f, ForceMode.Impulse);
            }
        }

        void IsKinematic(bool isKinematic)
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = isKinematic;
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

        public void ResetPlayer()
        {
            if (transform.position != Vector3.zero)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Vector3.zero, out hit, 100, NavMesh.AllAreas))
                {
                    navMeshAgent.Warp(hit.position);
                }
                else Debug.LogWarning("!");
            }
            weapon.ResetWeapon();
            IsKinematic(true);
            animator.enabled = true;
            animator.Rebind();
            ChangeLookAt();
            navMeshAgent.enabled = true;
            isKilling = false;
            transform.rotation = Quaternion.identity;
            col.enabled = true;
        }
    }
}
