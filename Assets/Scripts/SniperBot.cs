using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class SniperBot : SentryBot
    {
        public override IEnumerator Attack(GameObject target)
        {
            throw new System.NotImplementedException();
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            throw new System.NotImplementedException();
        }
    }
}
