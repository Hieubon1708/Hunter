using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Hunter
{
    public class GamePlay : MonoBehaviour
    {
        public TextMeshProUGUI textRemainingEnemy;

        public void UpdateRemainingEnemy()
        {
            textRemainingEnemy.text = "Enemy: " + GameController.instance.bots.Count;
        }

        public void Replay()
        {
            GameController.instance.isReseting = true;
            GameController.instance.Replay();
            DOVirtual.DelayedCall(0.5f, delegate
            {
                GameController.instance.isReseting = false;
            });
        }
    }
}
