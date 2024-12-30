using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter
{
    public class Health : MonoBehaviour
    {
        public Image healthDamagerBar;
        public Image healthBar;

        public Bot bot;

        public void ResetHealth()
        {
            healthBar.fillAmount = 1;
            healthDamagerBar.fillAmount = 1;
        }

        public void SubtractHp()
        {
            healthBar.DOComplete();
            healthDamagerBar.DOComplete();
            healthBar.DOFillAmount((float)bot.hp / bot.startHp, 0.25f);
            healthDamagerBar.DOFillAmount((float)bot.hp / bot.startHp, 0.25f).SetDelay(0.25f);
        }

        private void OnDestroy()
        {
            healthBar.DOComplete();
            healthDamagerBar.DOComplete();
        }
    }
}