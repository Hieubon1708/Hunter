using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public PlayerTouchMovement playerTouchMovement;

        public void Awake()
        {
            instance = this;
        }

        public float GetSpeed()
        {
            return playerTouchMovement.GetMovemntAmount().magnitude;
        }

        public void ResetGame()
        {
            if (transform.position != Vector3.zero)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Vector3.zero, out hit, 100, NavMesh.AllAreas))
                {
                    playerTouchMovement.navMeshAgent.Warp(hit.position);
                }
                else Debug.LogWarning("!");
            }
        }

        public void RandomPoppiesExtraRadius()
        {
            for (int i = 0; i < GameController.instance.poppies.Count; i++)
            {
                GameController.instance.poppies[i].xExtraRadius = Random.Range(-0.5f, 0.5f);
                GameController.instance.poppies[i].yExtraRadius = Random.Range(-0.5f, 0.5f);
            }
        }

        public void Lose()
        {
            playerTouchMovement.HandleLoseFinger();
        }

        public IEnumerator Win(Vector3 endPointPosition)
        {
            playerTouchMovement.navMeshAgent.destination = endPointPosition;
            playerTouchMovement.HandleLoseFinger();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => playerTouchMovement.navMeshAgent.remainingDistance == playerTouchMovement.navMeshAgent.stoppingDistance);
            UIController.instance.ChangeMap();
        }
    }
}
