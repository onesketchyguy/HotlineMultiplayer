using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    [CreateAssetMenu(menuName = "Weapon Dictionary")]
    public class WeaponDictionary : ScriptableObject
    {
        public WeaponObject[] weapons;

        public WeaponObject GetWeapon(int i)
        {
            if (i >= weapons.Length || i < 0) return null;
            return weapons[i];
        }

        public int GetWeaponIndex(WeaponObject weapon)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == weapon) return i;
            }

            return -1;
        }
    }
}