using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Hunter
{
    public class Health : MonoBehaviour
    {
        public int startHp;
        int hp;
        float startSizeBar;

        public Image healthDamagerBar;
        public Image healthBar;

        Tween subtractHealthBar;
        Tween subtractDamagerHealthBar;

        public void Start()
        {
            startSizeBar = healthBar.size.x;
            ResetHealth();
        }

        public void ResetHealth()
        {
            hp = startHp;
            healthBar.size = new Vector2(startSizeBar, healthBar.size.y);
            healthDamagerBar.size = new Vector2(startSizeBar, healthDamagerBar.size.y);
        }

        public void SubtractHp(int hp)
        {
            subtractHealthBar.Complete();
            subtractDamagerHealthBar.Complete();

            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            subtractHealthBar = DOVirtual.Float(healthBar.size.x, healthBar.size.x * this.hp / 100, 0.25f, (x) =>
            {
                healthBar.size = new Vector2(x, healthBar.size.y);
            });
            subtractDamagerHealthBar = DOVirtual.Float(healthDamagerBar.size.x, healthDamagerBar.size.x * this.hp / 100, 0.25f, (z) =>
            {
                healthDamagerBar.size = new Vector2(z, healthDamagerBar.size.y);
            }).SetDelay(0.15f);
        }
    }
}