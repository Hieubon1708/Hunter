using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : Bot
    {
        public override void FixedUpdate()
        {
            if (radarView.GetSeenVictim() != null)
            {
                if (!isFind)
                {
                    isFind = true;
                    StopProbe();
                    ChangeSpeed(pathInfo.detectSpeed, pathInfo.rotateDetectSpeed);
                    questionRotate.Show();
                }
                StopLostTrack();
                navMeshAgent.isStopped = true;
                GameObject target = radarView.GetSeenVictim().gameObject;
                transform.LookAt(target.transform.position);
                //Debug.LogWarning("Find " + target.name);
            }
            else
            {
                if (isFind)
                {
                    RaycastHit hit;
                    Vector3 direction = PlayerController.instance.transform.position - transform.position;
                    Physics.Raycast(transform.position, direction, out hit);
                    if (hit.collider.tag == "Player")
                    {
                        StopLostTrack();
                        animator.SetBool("Walking", true);
                        navMeshAgent.isStopped = false;
                        navMeshAgent.destination = PlayerController.instance.transform.position;
                        //Debug.LogWarning("Ray");
                    }
                    else
                    {
                        if (lostTrack == null)
                        {
                            lostTrack = StartCoroutine(LostTrack());
                        }
                    }
                }
            }
        }

        public override IEnumerator Attack()
        {
            while (true)
            {

            }
        }

        public override IEnumerator LostTrack()
        {
            Debug.LogWarning("Break");
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = PlayerController.instance.transform.position;
            animator.SetBool("Walking", true);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (true)
            {
                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(pathInfo.time);
            int step = Random.Range(2, 4);
            while (step > 0)
            {
                Vector3 randomDestination = RandomDestinationLostTrack();
                navMeshAgent.destination = randomDestination;
                animator.SetBool("Walking", true);
                //Debug.LogError("Position = " + randomDestination + " IsUpdatePosition = " + navMeshAgent.updatePosition + " Step = " + step);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                while (true)
                {
                    if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                    if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForSeconds(pathInfo.time);
                step--;
            }
            isFind = false;
            questionRotate.Hide();
            navMeshAgent.destination = pathInfo.paths[0][0];
            animator.SetBool("Walking", true);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (true)
            {
                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(pathInfo.time);
            StartProbe();
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                
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
