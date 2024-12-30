using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HieuBon
{
    public class CheckSizeCanvas : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CanvasScaler canvasScaler;

        void Start()
        {
            CheckSize();
        }

        public void CheckSize()
        {
            if (canvasScaler == null)
            {
                canvasScaler = GetComponent<CanvasScaler>();
            }
            if(cam == null) cam = Camera.main;
            canvasScaler.matchWidthOrHeight = cam.aspect < 1.818f ? 0 : 1;
        }
    }
}
