using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : Bot
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (radarView.GetSeenVictim() != null)
            {
                time = 0;
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
                GameObject target = radarView.GetSeenVictim().gameObject;
                transform.LookAt(target.transform.position);
                //Debug.LogWarning("Find " + target.name);
                if(attack == null)
                {
                    StartAttack();
                }
            }
            else
            {
                if (attack != null)
                {
                    StopAttack();
                }
                time += Time.fixedDeltaTime;
                if(time < 0.6f) return;
                if (isFind)
                {
                    radarView.SetColor(Color.white);
                    RaycastHit hit;
                    Vector3 direction = PlayerController.instance.transform.position - transform.position;
                    Physics.Raycast(transform.position, direction, out hit);
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
