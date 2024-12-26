using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public abstract class Bot : MonoBehaviour
    {
        public int startHp;
        public int damage;
        protected int hp;
        protected int indexPath;

        public ListeningDistance listeningDistance;
        public RadarView radarView;
        public QuestionRotate questionRotate;
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public PathInfo pathInfo;
        public ParticleSystem blood;

        public bool isFind;

        protected Coroutine probe;
        protected Coroutine attack;
        protected Coroutine lostTrack;
        protected Coroutine lastTrace;
        protected Coroutine hear;

        public GameObject scream;
        protected float time;

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            ResetBot();
            //StartProbe();
        }

        public void StopProbe()
        {
            if (probe != null) StopCoroutine(probe);
        }

        public void StartProbe()
        {
            probe = StartCoroutine(Probe());
        }

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

        public void StartHear(Vector3 position)
        {
            hear = StartCoroutine(Hear(position));
        }

        public void StopAttack()
        {
            if (attack != null)
            {
                animator.SetTrigger("NoAiming");
                StopCoroutine(attack);
                attack = null;
            }
        }

        public void StartAttack()
        {
            attack = StartCoroutine(Attack());
        }

        public void StartLostTrack()
        {
            lostTrack = StartCoroutine(LostTrack());
        }

        public void StartLastTrace()
        {
            lastTrace = StartCoroutine(LastTrace());
        }

        public void ChangeSpeed(float speed, float rotateSpeed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.speed = rotateSpeed;
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void SubtractHp(int hp)
        {
            this.hp -= hp;
        }

        IEnumerator Probe()
        {
            if (pathInfo.paths[0].Length > 1)
            {
                ChangeSpeed(pathInfo.speed, pathInfo.rotateSpeed);
                int index = 1;
                if (pathInfo.isUpdatePosition)
                {
                    if (pathInfo.pathType == GameController.PathType.Circle)
                    {
                        while (true)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, pathInfo.rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(pathInfo.time);
                            animator.SetBool("Walking", true);
                            navMeshAgent.destination = pathInfo.paths[indexPath][index];
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
                            if (index == pathInfo.paths[indexPath].Length - 1) index = 0;
                            else index++;
                        }
                    }
                    else if (pathInfo.pathType == GameController.PathType.Repeat)
                    {
                        bool isIncrease = true;
                        while (true)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, pathInfo.rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(pathInfo.time);
                            animator.SetBool("Walking", true);
                            navMeshAgent.destination = pathInfo.paths[indexPath][index];
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
                            if (index == pathInfo.paths[indexPath].Length - 1 || index == 0) isIncrease = !isIncrease;
                            if (isIncrease) index++;
                            else index--;
                        }
                    }
                }
                else
                {
                    if (pathInfo.pathType == GameController.PathType.Circle)
                    {
                        while (true)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, pathInfo.rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(pathInfo.time);
                            if (index == pathInfo.paths[indexPath].Length - 1) index = 1;
                            else index++;
                        }
                    }
                    else if (pathInfo.pathType == GameController.PathType.Repeat)
                    {
                        bool isIncrease = false;
                        while (true)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, pathInfo.rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(pathInfo.time);
                            if (index == pathInfo.paths[indexPath].Length - 1 || index == 1) isIncrease = !isIncrease;
                            if (isIncrease) index++;
                            else index--;
                        }
                    }
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(0, pathInfo.angle, 0);
                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, pathInfo.rotateSpeed);
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForSeconds(pathInfo.time);
            }
        }

        public IEnumerator Hear(Vector3 position)
        {
            isFind = true;
            navMeshAgent.destination = position;
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
            StartLostTrack();
        }

        public IEnumerator LostTrack()
        {
            Debug.LogWarning("LostTrack");
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

        public IEnumerator LastTrace()
        {
            Debug.LogWarning("LastTrace");
            navMeshAgent.isStopped = false;
            animator.SetBool("Walking", true);
            navMeshAgent.destination = PlayerController.instance.transform.position;
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
            StartLostTrack();
        }

        public IEnumerator Die()
        {
            scream.SetActive(true);
            yield return new WaitForFixedUpdate();
            scream.SetActive(false);
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                StartAttack();
            }
        }

        public IEnumerator Attack()
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            while (true)
            {
                yield return new WaitForSeconds(0.467f);
                PlayerController.instance.PlayBlood();
            }
        }

        public Vector3 RandomDestinationLostTrack()
        {
            float angle = Random.Range(0, 8) * 45f;
            Vector3 randomDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            int distance = Random.Range(3, 6);
            Debug.DrawLine(transform.position, transform.position + randomDirection * distance, Color.red, 111);
            //Debug.LogWarning("Angle = " + angle);
            //Debug.LogWarning("Distance = " + distance);
            return transform.position + randomDirection * distance;
        }

        public void PlayBlood()
        {
            blood.Play();
        }

        public void ResetBot()
        {
            indexPath = 0;
            hp = startHp;
            transform.position = pathInfo.paths[0][0];
            if (pathInfo.paths[0].Length == 1) transform.rotation = Quaternion.Euler(transform.eulerAngles.x, pathInfo.angle, transform.eulerAngles.z);
            else if (pathInfo.paths[0].Length > 1) transform.LookAt(pathInfo.paths[0][1]);
        }
    }
}