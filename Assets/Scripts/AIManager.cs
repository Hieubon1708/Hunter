using UnityEngine;

namespace Hunter
{
    [DefaultExecutionOrder(0)]
    public class AIManager : MonoBehaviour
    {
        private static AIManager _instance;
        public static AIManager Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public Transform Target;
        public float RadiusAroundTarget = 0.5f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }

            Destroy(gameObject);
        }

        public void Update()
        {
            MakeAgentsCircleTarget();
        }

        private void MakeAgentsCircleTarget()
        {
            if (GameController.instance.poppies.Count == 0) return;
            if (GameController.instance.poppies.Count == 1)
            {
                for (int i = 0; i < GameController.instance.poppies.Count; i++)
                {
                    GameController.instance.poppies[i].aIUnit.MoveTo(Target.position, RadiusAroundTarget);
                }
            }
            else
            {
                float mulRadius = 0;
                for (int i = 0; i < GameController.instance.poppies.Count; i++)
                {
                    float mulI = i;
                    if (i >= 4)
                    {
                        mulRadius = 0.75f;
                        mulI = mulI - 4 + 0.5f;
                    }
                    if (i >= 8)
                    {
                        mulRadius = 1f;
                        mulI = mulI - 8 + 0.5f;
                    }
                    GameController.instance.poppies[i].aIUnit.MoveTo(new Vector3(
                   Target.position.x + (RadiusAroundTarget + mulRadius + GameController.instance.poppies[i].aIUnit.extraX) * Mathf.Cos(2 * Mathf.PI * mulI / 4),
                   Target.position.y,
                   Target.position.z + +(RadiusAroundTarget + mulRadius + GameController.instance.poppies[i].aIUnit.extraY) * Mathf.Sin(2 * Mathf.PI * mulI / 4)
                   ), RadiusAroundTarget + mulRadius);
                }
            }
        }
    }
}
