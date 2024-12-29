using Unity.AI.Navigation;
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
        public float extraX;
        public float extraY;
        bool isXIncrease;
        bool isYIncrease;

        private void Start()
        {
            RandomExtra();
        }

        void RandomExtra()
        {
            isXIncrease = Random.Range(0, 2) == 1;
            isYIncrease = Random.Range(0, 2) == 1;

            Invoke("RandomExtra", Random.Range(0.5f, 1f));
        }

        public void MoveTo(Vector3 Position, float radius)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(Position, out hit, 0.1f, NavMesh.AllAreas))
            {
                Agent.stoppingDistance = 0;
                Agent.destination = Position;
            }
            else
            {
                Agent.stoppingDistance = radius;
                Agent.destination = AIManager.Instance.Target.position;
            }
        }

        public void Update()
        {
            if (PlayerController.instance != null)
            {
                float touch = PlayerController.instance.GetSpeed();
                float speed = touch == 0 ? Agent.velocity.magnitude : touch * Agent.speed / 3;
                animator.SetFloat("Speed", Mathf.Clamp01(speed));
                float mouseMagnitude = PlayerController.instance.playerTouchMovement.scaledMovement.magnitude / 5;
                extraX = Mathf.Clamp(isXIncrease ? extraX + mouseMagnitude : extraX - mouseMagnitude, -1f, 1f);
                extraY = Mathf.Clamp(isYIncrease ? extraY + mouseMagnitude : extraY - mouseMagnitude, -1f, 1f);
            }
        }
    }
}
