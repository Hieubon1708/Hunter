using UnityEngine;

namespace Hunter
{
    public static class LayerUtils
    {
        public static int GetRadarAllLayerMask()
        {
            return ~LayerMask.GetMask("Bot", "Player");
        }

        public static int GetVictimLayerMask()
        {
            return LayerMask.GetMask("Player");
        }
    }
}