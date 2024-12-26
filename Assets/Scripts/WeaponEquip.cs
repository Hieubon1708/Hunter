using UnityEngine;

namespace Hunter
{
    public class WeaponEquip : MonoBehaviour
    {
        public Weapon scWeapon;
        public GameObject weapon;
        public GameObject[] preWeapons;

        private void Start()
        {
            Equip(GameController.WeaponType.Knife);
        }

        public void Equip(GameController.WeaponType weaponType)
        {
            if (weapon != null) Destroy(weapon);
            GameObject w = GetPreWeaponByName(weaponType.ToString());
            if (w)
            {
                weapon = Instantiate(w, PlayerController.instance.hand);
                scWeapon = weapon.GetComponent<Weapon>();
                PlayerController.instance.Init(scWeapon.attackRange);
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