using UnityEngine;

namespace Hunter
{
    public class PathInfo : MonoBehaviour
    {
        public GameController.BotType botType;
        public GameController.PathType pathType;
        [Range(1f, 3f)]
        public float speed;
        public float time;
        public Transform[] path;
        public Vector3[][] paths;
        public bool isUpdatePosition = true;
        public float angle;

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            paths = new Vector3[path.Length][];
            for (int i = 0; i < paths.Length; i++)
            {
                Vector3[] pathChild = new Vector3[path[i].childCount];
                for (int j = 0; j < pathChild.Length; j++)
                {
                    if (i == 0 && j == 0) angle = path[i].GetChild(j).transform.eulerAngles.y;
                    Vector3 pos = path[i].GetChild(j).transform.position;
                    pathChild[j] = new Vector3(pos.x , 1.083333f, pos.z);
                }
                paths[i] = pathChild;
            }
            GameController.instance.SetBot(botType, this);
        }
    }
}
