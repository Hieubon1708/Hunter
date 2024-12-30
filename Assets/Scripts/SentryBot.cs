using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public abstract class SentryBot : Bot
    {
        public ListeningDistance listeningDistance;
        public QuestionRotate questionRotate;

        protected Coroutine lostTrack;
        protected Coroutine lastTrace;
        protected Coroutine hear;

        public LayerMask playerLayer;

        public GameObject scream;

        public void StopLostTrack()
        {
            if (lostTrack != null) StopCoroutine(lostTrack);
        }

        public void StopLastTrace()
        {
            if (lastTrace != null)
            {
                StopCoroutine(lastTrace);
                lastTrace = null;
            }
        }

        public void StopHear()
        {
            if (hear != null)
            {
                StopCoroutine(hear);
                hear = null;
            }
        }

        public void StartHear(GameObject target)
        {
            hear = StartCoroutine(Hear(target));
        }

        public void StartLostTrack(GameObject target)
        {
            lostTrack = StartCoroutine(LostTrack(target));
        }

        public void StartLastTrace(GameObject target)
        {
            lastTrace = StartCoroutine(LastTrace(target));
        }


        public IEnumerator Hear(GameObject target)
        {
            isFind = true;
            navMeshAgent.destination = target.transform.position;
            questionRotate.Show();
            animator.SetBool("Walking", true);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (true)
            {
                if (navMeshAgent.remainingDistance <= 1.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1) break;
                yield return new WaitForFixedUpdate();
            }
            StartLostTrack(target);
        }


        public IEnumerator LostTrack(GameObject target)
        {
            Debug.LogWarning("LostTrack");
            Player player = GameController.instance.GetPoppy(target);
            if (player != null) navMeshAgent.destination = player.transform.position;
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
                while (true)
                {
                    List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
                    int indexRandom = Random.Range(0, list.Count);
                    float angle = list[indexRandom] * 45f;
                    Vector3 randomDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                    float distance = Random.Range(3f, 6f);
                    Debug.DrawLine(transform.position, transform.position + randomDirection * distance, Color.red, 111);
                    navMeshAgent.destination = transform.position + randomDirection * distance;
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    if (navMeshAgent.remainingDistance <= 5) break;
                    else list.RemoveAt(indexRandom);
                }
                animator.SetBool("Walking", true);
                //Debug.LogError("Position = " + randomDestination + " IsUpdatePosition = " + navMeshAgent.updatePosition + " Step = " + step);
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

        public IEnumerator LastTrace(GameObject target)
        {
            Debug.LogWarning("LastTrace");
            Player player = GameController.instance.GetPoppy(target);
            navMeshAgent.isStopped = false;
            if (player != null)
            {
                animator.SetBool("Walking", true);
                navMeshAgent.destination = player.transform.position;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                while (true)
                {
                    if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                    if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                    yield return new WaitForFixedUpdate();
                }
            }
            yield return new WaitForSeconds(pathInfo.time);
            StartLostTrack(target);
        }

        public IEnumerator Die()
        {
            scream.SetActive(true);
            yield return new WaitForFixedUpdate();
            scream.SetActive(false);
        }

        public override void ResetBot()
        {
            isFind = false;
            StopAttack();
            StopHear();
            StopLastTrace();
            StopLostTrack();
            StopProbe();
            radarView.SetColor(new Vector4(1, 1, 1, 70f / 255f));
            questionRotate.transform.localScale = Vector3.zero;
            hp = startHp;
            indexPath = 0;
            IsKinematic(true);
            col.enabled = true;
            animator.enabled = true;
            animator.Rebind();
            isKilling = false;
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            radarView.gameObject.SetActive(true);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pathInfo.paths[0][0], out hit, 100, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
            else Debug.LogWarning("!");
            if (pathInfo.paths[0].Length == 1) transform.rotation = Quaternion.Euler(transform.eulerAngles.x, pathInfo.angle, transform.eulerAngles.z);
            else if (pathInfo.paths[0].Length > 1) transform.LookAt(pathInfo.paths[0][1], Vector3.up);
            navMeshAgent.destination = transform.position;
            weapon.ResetWeapon();
        }
    }
}
