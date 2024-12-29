using UnityEngine;

namespace Hunter
{
    public static class LayerUtils
    {
        public static int GetRadarAllLayerMask()
        {
            return ~LayerMask.GetMask("Bot", "Player", "AttackRange", "PlayerBone");
        }

        public static int GetVictimLayerMask()
        {
            return LayerMask.GetMask("Player");
        }
    }
}