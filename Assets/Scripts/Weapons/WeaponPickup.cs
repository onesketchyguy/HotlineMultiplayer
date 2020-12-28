using UnityEngine;

namespace TopDownShooter
{
    public class WeaponPickup : ItemPickup
    {
        public WeaponObject weapon;

        protected override void OnPlayerPickup(GameObject sender)
        {
            base.OnPlayerPickup(sender);

            // Pickup this weapon
            var weaponManager = sender.GetComponent<WeaponManager>();

            if (weaponManager)
                weaponManager.EquipWeapon(weapon);
        }
    }
}