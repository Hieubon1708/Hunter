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

        int indexPath;

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
            ResetBot();
            StartCoroutine(Probe());
        }

        IEnumerator Probe()
        {
            int index = 1;
            while (true)
            {
                navMeshAgent.destination = pathInfo.paths[indexPath][index];
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                yield return new WaitUntil(() => navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance);
                if (index == pathInfo.paths[indexPath].Length - 1) index = 0;
                else index++;
            }
        }

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
        }
    }
}