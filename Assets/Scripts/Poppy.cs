using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class Poppy : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;
        public Animator animator;

        public void LateUpdate()
        {
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            //if(Player.instance != null) navMeshAgent.destination = Player.instance.transform.position;
        }
    }
}
