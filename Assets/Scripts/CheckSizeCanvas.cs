using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HieuBon
{
    public class CheckSizeCanvas : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CanvasScaler canvasScaler;

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            CheckSize();
        }

        public void CheckSize()
        {
            if (canvasScaler == null)
            {
                canvasScaler = GetComponent<CanvasScaler>();
            }
            canvasScaler.matchWidthOrHeight = cam.aspect < 1.818f ? 0 : 1;
        }
    }
}
