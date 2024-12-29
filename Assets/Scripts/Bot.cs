using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public abstract class Bot : MonoBehaviour
    {
        public int startHp;
        protected int hp;
        protected int indexPath;

        public ListeningDistance listeningDistance;
        public RadarView radarView;
        public QuestionRotate questionRotate;
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
        protected Coroutine lostTrack;
        protected Coroutine lastTrace;
        protected Coroutine hear;

        public LayerMask playerLayer;

        public GameObject scream;
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

        public void StartLostTrack(GameObject target)
        {
            lostTrack = StartCoroutine(LostTrack(target));
        }

        public void StartLastTrace(GameObject target)
        {
            lastTrace = StartCoroutine(LastTrace(target));
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
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            PlayBlood();
            if (this.hp <= 0)
            {
                UIController.instance.HitEffect();

                StopHear();
                StopProbe();
                StopAttack();
                StopLastTrace();
                StopLostTrack();

                isFind = false;
                col.enabled = false;
                animator.enabled = false;
                navMeshAgent.enabled = false;
                radarView.gameObject.SetActive(false);
                questionRotate.Hide();
                IsKinematic(false);

                Vector3 dir = transform.position - PlayerController.instance.transform.position;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y + 1, dir.z) * 7, ForceMode.Impulse);
                }

                GameController.instance.RemoveBot(gameObject);
                UIController.instance.gamePlay.UpdateRemainingEnemy();
            }
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

        public abstract IEnumerator Attack(GameObject target);

        void IsKinematic(bool isKinematic)
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

        public void ResetBot()
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                // do something
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                // do something
            }
        }
    }
}