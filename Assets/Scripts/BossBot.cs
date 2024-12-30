using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class BossBot : Bot
    {
        public GameObject arrow;
        Coroutine run;
        public Health health;
        public int amountBullet;
        public GameObject preBullet;
        public Bullet[] bullets;
        int indexBullet;
        public float speed;
        bool isRuning;

        public void Start()
        {
            UIController.instance.gamePlay.textRemainingEnemy.gameObject.SetActive(false);
            bullets = new Bullet[amountBullet];
            for (int i = 0; i < amountBullet; i++)
            {
                GameObject b = Instantiate(preBullet, GameController.instance.poolWeapon);
                b.SetActive(false);
                bullets[i] = b.GetComponent<Bullet>();
                bullets[i].bot = this;
            }
        }

        public void FixedUpdate()
        {
            if (!col.enabled || GameController.instance.isResarting || isRuning) return;
            GameObject target = null;
            if (radarView.GetSeenVictim() != null)
            {
                if (!navMeshAgent.isStopped)
                {
                    StopProbe();
                    radarView.SetColor(Color.red);
                    navMeshAgent.isStopped = true;
                    animator.SetBool("Walking", false);
                    target = radarView.GetSeenVictim().gameObject;
                    transform.LookAt(target.transform.position);
                    //Debug.LogError("Find " + target.name);
                    Debug.LogWarning("Start");
                    StartAttack(target);
                }

            }
            else
            {
                if (!isKilling)
                {
                    if (navMeshAgent.isStopped)
                    {
                        StopAttack();
                        StartProbe(index);
                        radarView.SetColor(new Vector4(1, 1, 1, 70f / 255f));
                        navMeshAgent.isStopped = false;
                        animator.SetBool("Walking", true);
                    }
                }
            }
        }

        public override IEnumerator Attack(GameObject poppy)
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            int dir = -1;
            for (int i = 0; i < 3; i++)
            {
                bullets[indexBullet].transform.position = weapon.transform.position;
                bullets[indexBullet].transform.LookAt(new Vector3(poppy.transform.position.x + (dir + i), weapon.transform.position.y, poppy.transform.position.z));
                bullets[indexBullet].gameObject.SetActive(true);
                indexBullet++;
                if (indexBullet == bullets.Length) indexBullet = 0;
            }
            yield return new WaitForSeconds(0.467f);
            StopAttack();
        }

        void StartRun(Transform killer)
        {
            if (run == null)
            {
                run = StartCoroutine(Run(killer));
            }
        }

        void StopRun()
        {
            if (run != null)
            {
                StopCoroutine(run);
                run = null;
            }
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            PlayBlood();
            StopProbe();
            StopAttack();
            StopRun();
            health.SubtractHp();
            if (this.hp <= 0)
            {
                GameController.instance.poppyTypes.Add(GameController.instance.RandomPoppy());
                PlayerPrefs.SetInt("HunterLevel", 1);
                UIController.instance.camAni.Play("CamBossZoom");
                PlayerController.instance.playerTouchMovement.HandleLoseFinger();
                UIController.instance.gamePlay.layerCover.SetActive(true);
                UIController.instance.HitEffect();
                col.enabled = false;
                animator.enabled = false;
                navMeshAgent.enabled = false;
                radarView.gameObject.SetActive(false);
                arrow.SetActive(false);
                IsKinematic(false);
                Invoke("BossEnd", 3f);
                Vector3 dir = transform.position - PlayerController.instance.transform.position;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y + 1, dir.z) * 7, ForceMode.Impulse);
                }

                GameController.instance.RemoveBot(gameObject);

            }
            else
            {
                StartRun(killer);
            }
        }

        void BossEnd()
        {
            GameController.instance.poppyTypes.Add(GameController.instance.RandomPoppy());
            UIController.instance.BossEnd();
        }

        IEnumerator Run(Transform killer)
        {
            Debug.LogWarning("Run");
            isRuning = true;
            indexPath++;
            navMeshAgent.isStopped = false;
            radarView.SetColor(new Vector4(1, 1, 1, 70f / 255f));
            ChangeSpeed(pathInfo.detectSpeed, pathInfo.rotateDetectSpeed);
            Vector3 dirOfAttack = transform.position - killer.position;
            navMeshAgent.destination = transform.position + dirOfAttack * 2;
            animator.SetBool("Walking", true);
            animator.SetTrigger("Dodging");
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (col.enabled)
            {
                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                yield return new WaitForFixedUpdate();
            }
            navMeshAgent.destination = pathInfo.paths[indexPath][0];
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (col.enabled)
            {
                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(pathInfo.time);
            StartProbe(1);
            isRuning = false;
        }

        public override void ResetBot()
        {
            StopAttack();
            StopProbe();
            StopRun();
            radarView.SetColor(new Vector4(1, 1, 1, 70f / 255f));
            arrow.SetActive(true);
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
            health.ResetHealth();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pathInfo.paths[0][0], out hit, 100, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
            else Debug.LogWarning("!");
            transform.LookAt(pathInfo.paths[0][1], Vector3.up);
            navMeshAgent.destination = transform.position;
            weapon.ResetWeapon();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                Destroy(bullets[i]);
            }
        }
    }
}
