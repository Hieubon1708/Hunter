using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public abstract class Bot : MonoBehaviour
    {
        public int startHp;
        protected int hp;
        protected int indexPath;

        public RadarView radarView;
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public PathInfo pathInfo;
        public ParticleSystem blood;
        public CapsuleCollider col;
        public Transform hips;
        public Rigidbody[] rbs;

        public bool isFind;
        protected float timeOff;
        public bool isKilling;

        protected Coroutine probe;
        protected Coroutine attack;

        public BotWeapon weapon;

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
        }

        public void StopProbe()
        {
            if (probe != null) StopCoroutine(probe);
        }

        public void StartProbe()
        {
            probe = StartCoroutine(Probe());
        }

        public void StopAttack()
        {
            if (attack != null)
            {
                isKilling = false;
                animator.SetTrigger("NoAiming");
                StopCoroutine(attack);
                attack = null;
            }
        }

        public void StartAttack(GameObject target)
        {
            if (isKilling) return;
            animator.ResetTrigger("NoAiming");
            animator.ResetTrigger("Fire");
            attack = StartCoroutine(Attack(target));
        }

        public void ChangeSpeed(float speed, float rotateSpeed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.speed = rotateSpeed;
        }

        public abstract void SubtractHp(int hp);

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


        public abstract IEnumerator Attack(GameObject target);

        public void IsKinematic(bool isKinematic)
        {
            if (rbs.Length == 0) rbs = hips.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = isKinematic;
            }
        }

        public void PlayBlood()
        {
            blood.Play();
        }

        public abstract void ResetBot();
    }
}