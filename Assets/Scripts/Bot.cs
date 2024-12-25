using DG.Tweening;
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
        public ListeningDistance listeningDistance;
        public GameObject scream;

        public NavMeshAgent navMeshAgent;
        public PathInfo pathInfo;

        public bool isFind;
        int indexPath;
        Coroutine probe;
        protected Coroutine attack;
        protected Coroutine lostTrack;

        public RadarView radarView;
        public Animator animator;

        public QuestionRotate questionRotate;

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            ResetBot();
            StartProbe();
        }

        public void StopProbe()
        {
            if (probe != null) StopCoroutine(probe);
        }

        public void StartProbe()
        {
            probe = StartCoroutine(Probe());
        }

        public void ChangeSpeed(float speed, float rotateSpeed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.speed = rotateSpeed;
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

        public abstract void FixedUpdate();

        public abstract IEnumerator Attack();

        public abstract IEnumerator LostTrack();

        public virtual void SubtractHp(int hp)
        {
            this.hp -= hp;
        }

        public void StopLostTrack()
        {
            if (lostTrack != null)
            {
                StopCoroutine(lostTrack);
                lostTrack = null;
            }
        }

        public Vector3 RandomDestinationLostTrack()
        {
            float angle = Random.Range(0, 8) * 45f;
            Vector3 randomDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            int distance = Random.Range(3, 6);
            //Debug.LogWarning("Angle = " + angle);
            //Debug.LogWarning("Distance = " + distance);
            Debug.LogWarning(transform.position);
            Debug.DrawLine(transform.position, transform.position + randomDirection * distance, Color.red, 111);
            return transform.position + randomDirection * distance;
        }

        public IEnumerator Die()
        {
            scream.SetActive(true);
            yield return new WaitForFixedUpdate();
            scream.SetActive(false);
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