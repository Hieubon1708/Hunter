using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : SentryBot
    {
        public ParticleSystem parWeapon;
        public GameObject laserWeapon;

        public void FixedUpdate()
        {
            if (!col.enabled || GameController.instance.isResarting) return;
            GameObject target = null;
            if (radarView.GetSeenVictim() != null)
            {
                timeOff = 0;
                if (!isFind)
                {
                    isFind = true;
                    StopProbe();
                    ChangeSpeed(pathInfo.detectSpeed, pathInfo.rotateDetectSpeed);
                    questionRotate.Show();
                }
                StopHear();
                StopLastTrace();
                StopLostTrack();
                radarView.SetColor(Color.red);
                navMeshAgent.isStopped = true;
                animator.SetBool("Walking", false);
                target = radarView.GetSeenVictim().gameObject;
                transform.LookAt(target.transform.position);
                //Debug.LogError("Find " + target.name);
                if (attack == null)
                {
                    //Debug.LogWarning("Start");
                    StartAttack(target);
                }
            }
            else
            {
                if (isFind)
                {
                    if (attack != null)
                    {
                        //Debug.LogError("Stop");
                        StopAttack();
                    }
                    radarView.SetColor(new Vector4(1, 1, 1, 70f / 255f));
                    timeOff += Time.fixedDeltaTime;
                    if (timeOff < 0.6f) return;
                    RaycastHit hit;
                    Vector3 from = transform.position;
                    Vector3 to = PlayerController.instance.transform.position;
                    from.y += 0.1f;
                    to.y += 0.1f;
                    Vector3 direction = to - from;
                    Physics.Raycast(from, direction, out hit, 10, playerLayer);
                    if (hit.collider != null && hit.collider.tag == "Player")
                    {
                        StopHear();
                        StopLastTrace();
                        navMeshAgent.isStopped = false;
                        animator.SetBool("Walking", true);
                        navMeshAgent.destination = PlayerController.instance.transform.position;
                        //Debug.LogWarning("Ray");
                    }
                    else
                    {
                        if (lastTrace == null && hear == null)
                        {
                            StartLastTrace(target);
                        }
                    }
                }
            }
        }

        public override IEnumerator Attack(GameObject target)
        {
            isKilling = true;
            Player player = GameController.instance.GetPoppy(target);
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            while (player.col.enabled)
            {
                parWeapon.Play();
                player.PlayBlood();
                player.Die(transform);
                yield return new WaitForSeconds(0.467f);
            }
            StopAttack();
        }

        public override void SubtractHp(int hp)
        {
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            if (this.hp <= 0)
            {
                StopHear();
                StopProbe();
                StopAttack();
                StopLastTrace();
                StopLostTrack();
                PlayBlood();

                UIController.instance.HitEffect();
                UIController.instance.ShakeCam();
                isFind = false;
                col.enabled = false;
                animator.enabled = false;
                navMeshAgent.enabled = false;
                radarView.gameObject.SetActive(false);
                questionRotate.Hide();
                IsKinematic(false);
                StartCoroutine(Die());

                Vector3 dir = transform.position - PlayerController.instance.transform.position;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y + 1, dir.z) * 7, ForceMode.Impulse);
                }

                GameController.instance.RemoveBot(gameObject);
                UIController.instance.gamePlay.UpdateRemainingEnemy();
            }
        }
    }
}
