using UnityEngine;

namespace Hunter
{
    public class WeaponEquip : MonoBehaviour
    {
        public GameObject[] preWeapons;

        public void Equip(Player player, GameController.WeaponType weaponType)
        {
            if (player.weapon != null) Destroy(player.weapon);
            GameObject w = GetPreWeaponByName(weaponType.ToString());
            if (w)
            {
                GameObject weapon = Instantiate(w, player.hand);
                player.Init(weapon.GetComponent<Weapon>());
            }
            else
            {
                Debug.LogError("Not found!");
            }
        }

        GameObject GetPreWeaponByName(string name)
        {
            for (int i = 0; i < preWeapons.Length; i++)
            {
                if (preWeapons[i].name.ToLower() == name.ToLower()) return preWeapons[i];
            }
            return null;
        }
    }
}