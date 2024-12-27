using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : Bot
    {
        public ParticleSystem parWeapon;
        public GameObject laserWeapon;

        public override void FixedUpdate()
        {
            if(!col.enabled) return;
            base.FixedUpdate();
            if (radarView.GetSeenVictim() != null)
            {
                timeOff = 0;
                if (!isFind)
                {
                    isFind = true;
                    StopProbe();
                    ChangeSpeed(pathInfo.detectSpeed, pathInfo.rotateDetectSpeed);
                    questionRotate.Show();
                }
                StopHear();
                StopLastTrace();
                radarView.SetColor(Color.red);
                navMeshAgent.isStopped = true;
                animator.SetBool("Walking", false);
                GameObject target = radarView.GetSeenVictim().gameObject;
                transform.LookAt(target.transform.position);
                //Debug.LogWarning("Find " + target.name);
                if (attack == null)
                {
                    //Debug.LogWarning("Start");
                    StartAttack();
                }
            }
            else
            {
                if (isFind)
                {
                    if (attack != null)
                    {
                        //Debug.LogError("Stop");
                        StopAttack();
                    }
                    radarView.SetColor(Color.white);
                    timeOff += Time.fixedDeltaTime;
                    if (timeOff < 0.6f) return;
                    RaycastHit hit;
                    Vector3 from = transform.position;
                    Vector3 to = PlayerController.instance.transform.position;
                    from.y += 0.1f;
                    to.y += 0.1f;
                    Vector3 direction = to - from;
                    Physics.Raycast(from, direction, out hit, 10, playerLayer);
                    if (hit.collider != null && hit.collider.tag == "Player")
                    {
                        StopHear();
                        StopLastTrace();
                        navMeshAgent.isStopped = false;
                        animator.SetBool("Walking", true);
                        navMeshAgent.destination = PlayerController.instance.transform.position;
                        //Debug.LogWarning("Ray");
                    }
                    else
                    {
                        if (lastTrace == null && hear == null)
                        {
                            StartLastTrace();
                        }
                    }
                }
            }
        }

        public override IEnumerator Attack()
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            while (true)
            {
                parWeapon.Play();
               /* PlayerController.instance.PlayBlood();
                PlayerController.instance.SubtractHp(damage);*/
                yield return new WaitForSeconds(0.467f);
            }
        }

        public override void SubtractHp(int hp)
        {
            base.SubtractHp(hp);
            if (this.hp <= 0)
            {
                Debug.LogWarning("Die");
                StartCoroutine(Die());
            }
        }
    }
}
