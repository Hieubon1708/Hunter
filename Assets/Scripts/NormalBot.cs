using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : Bot
    {
        public override void Find()
        {
            base.Find();
        }

        public override IEnumerator Attack()
        {
            while (true)
            {

            }
        }

        public override void SubtractHp(int hp)
        {
            base.SubtractHp(hp);
            if (this.hp <= 0)
            {
                Debug.LogWarning("Die");
                StartCoroutine(Die());
            }
        }
    }
}
