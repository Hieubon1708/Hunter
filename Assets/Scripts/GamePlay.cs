using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Hunter
{
    public class GamePlay : MonoBehaviour
    {
        public TextMeshProUGUI textRemainingEnemy;
        public GameObject layerCover;
        public GameObject panelStart;
        public GameObject buttonReplay;

        public void UpdateRemainingEnemy()
        {
            if(!textRemainingEnemy.gameObject.activeSelf) textRemainingEnemy.gameObject.SetActive(true);
            textRemainingEnemy.text = "Enemy: " + GameController.instance.bots.Count;
        }

        public void Replay()
        {
            buttonReplay.SetActive(false);
            GameController.instance.isResarting = true;
            GameController.instance.Replay();
            UIController.instance.cam.m_Lens.FieldOfView = UIController.instance.defaultFieldOfView;
            PlayerController.instance.playerTouchMovement.HideTouch();
            UIController.instance.LoadUI();
            panelStart.SetActive(true);
            DOVirtual.DelayedCall(0.02f, delegate { GameController.instance.isResarting = false; });
        }

        public void Restart()
        {
            panelStart.SetActive(true);
            layerCover.SetActive(false);
            UIController.instance.cam.m_Lens.FieldOfView = UIController.instance.defaultFieldOfView;
            PlayerController.instance.playerTouchMovement.HideTouch();
        }
    }
}
