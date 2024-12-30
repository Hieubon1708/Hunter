using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class BossBot : Bot
    {
        public GameObject arrow;
        Coroutine run;

        public void Start()
        {
            UIController.instance.gamePlay.textRemainingEnemy.gameObject.SetActive(false);
        }

        public override IEnumerator Attack(GameObject poppy)
        {
            throw new System.NotImplementedException();
        }

        void StartRun()
        {
            run = StartCoroutine(Run());
        }

        void StopRun()
        {
            if(run != null) StopCoroutine(run);
        }

        public override void SubtractHp(int hp)
        {
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            PlayBlood();
            StopProbe();
            StopAttack();
            if (this.hp <= 0)
            {
                UIController.instance.camAni.Play("CamBossZoom");
                PlayerController.instance.playerTouchMovement.HandleLoseFinger();
                UIController.instance.gamePlay.layerCover.SetActive(true);
                UIController.instance.HitEffect();
                isFind = false;
                col.enabled = false;
                animator.enabled = false;
                navMeshAgent.enabled = false;
                radarView.gameObject.SetActive(false);
                arrow.SetActive(false);
                IsKinematic(false);

                Vector3 dir = transform.position - PlayerController.instance.transform.position;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y + 1, dir.z) * 7, ForceMode.Impulse);
                }

                GameController.instance.RemoveBot(gameObject);

            }
            else
            {
                StartRun();
            }
        }

        void BossEnd()
        {
            UIController.instance.BossEnd();
        }

        IEnumerator Run()
        {
            indexPath++;
            ChangeSpeed(pathInfo.detectSpeed, pathInfo.rotateDetectSpeed);
            navMeshAgent.destination = pathInfo.paths[indexPath][0];
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

        public override void ResetBot()
        {
            isFind = false;
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
    }
}
