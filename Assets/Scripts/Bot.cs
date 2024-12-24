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

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            ResetBot();
            probe = StartCoroutine(Probe());
        }

        public void StopProbe()
        {
            if (probe != null) StopCoroutine(probe);
            navMeshAgent.isStopped = true;
        }

        IEnumerator Probe()
        {
            if (pathInfo.paths[0].Length > 1)
            {
                navMeshAgent.updatePosition = pathInfo.isUpdatePosition;
                int index = 1;
                if (pathInfo.pathType == GameController.PathType.Circle)
                {
                    while (true)
                    {
                        navMeshAgent.destination = pathInfo.paths[indexPath][index];
                        yield return new WaitForFixedUpdate();
                        yield return new WaitForFixedUpdate();
                        yield return new WaitForFixedUpdate();
                        yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
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
                        navMeshAgent.destination = pathInfo.paths[indexPath][index];
                        yield return new WaitForFixedUpdate();
                        yield return new WaitForFixedUpdate();
                        yield return new WaitForFixedUpdate();
                        yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
                        yield return new WaitForSeconds(pathInfo.time);
                        if (index == pathInfo.paths[indexPath].Length - 1 || index == 0) isIncrease = !isIncrease;
                        if (isIncrease) index++;
                        else index--;
                    }
                }
            }
        }

        public virtual void Find()
        {
            isFind = true;
            StopProbe();
        }
        public abstract IEnumerator Attack();

        public virtual void SubtractHp(int hp)
        {
            this.hp -= hp;
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
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, pathInfo.angle, transform.eulerAngles.z);
        }
    }
}