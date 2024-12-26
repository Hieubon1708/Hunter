using UnityEngine;

namespace Hunter
{
    public class WeaponEquip : MonoBehaviour
    {
        public GameObject weapon;
        public GameObject[] preWeapons;
        public Transform container;

        public void Equip(GameObject weapon, GameController.WeaponType weaponType)
        {
            if (weapon != null) Destroy(weapon);
            if (GetPreWeaponByName(weapon.ToString()))
            {
                weapon = Instantiate(weapon, container);
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