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
                }
                StopLostTrack();
                navMeshAgent.isStopped = true;
                navMeshAgent.updatePosition = true;
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

        public GameObject a;

        public override IEnumerator LostTrack()
        {
            Debug.LogWarning("Break");
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = PlayerController.instance.transform.position;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
            yield return new WaitForSeconds(pathInfo.time);
            int step = Random.Range(3, 6);
            while (step > 0)
            {
                Vector3 randomDestination = RandomDestinationLostTrack();
                navMeshAgent.destination = randomDestination;
                //Instantiate(a, randomDestination, Quaternion.identity);
                navMeshAgent.updatePosition = Random.Range(0, 5) == 0;
                Debug.LogError("Position = " + randomDestination + " IsUpdatePosition = " + navMeshAgent.updatePosition + " Step = " + step);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
                yield return new WaitForSeconds(pathInfo.time);
                navMeshAgent.nextPosition = transform.position;
                step--;
            }
            isFind = false;
            navMeshAgent.destination = pathInfo.paths[0][0];
            navMeshAgent.updatePosition = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
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
