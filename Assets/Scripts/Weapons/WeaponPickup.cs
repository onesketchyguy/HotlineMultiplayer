using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class WeaponPickup : ItemPickup
    {
        public WeaponObject weapon;

        protected override void OnPlayerPickup(GameObject sender)
        {
            // Pickup this weapon
            var weaponManager = sender.GetComponent<WeaponManager>();

            if (weaponManager)
            {
                weaponManager.EquipWeapon(this);
            }
        }
    }
}