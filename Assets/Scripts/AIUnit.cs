using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    [RequireComponent(typeof(NavMeshAgent))]
    [DefaultExecutionOrder(1)]
    public class AIUnit : MonoBehaviour
    {
        public NavMeshAgent Agent;
        public Animator animator;

        private void Awake()
        {
            AIManager.Instance.Units.Add(this);
        }

        public void MoveTo(Vector3 Position)
        {
            Agent.destination = Position;
        }

        public void FixedUpdate()
        {
            animator.SetFloat("Speed", Agent.velocity.magnitude);
        }
    }
}
