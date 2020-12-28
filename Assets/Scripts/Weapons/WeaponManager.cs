using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class WeaponManager : NetworkBehaviour
    {
        public CombatController controller;

        public SpriteRenderer weaponDisplay;

        public WeaponObject equippedWeapon;

        public float defaultMeleeRange = 1;
        public float defaultFireRate = 0.5f;

        [Server]
        public void EquipWeapon(WeaponObject weapon)
        {
            CmdDropWeapon();

            equippedWeapon = weapon;
            CmdEquipWeapon();
        }

        // Called on server
        [Command]
        public void CmdEquipWeapon()
        {
            RpcEquipWeapon();
        }

        [Command]
        public void CmdDropWeapon()
        {
            RpcDropWeapon();
        }

        [ClientRpc]
        public void RpcDropWeapon()
        {
            equippedWeapon = null;

            // We should drop a weapon here! TO BE IMPLEMENTED

            CmdEquipWeapon();
        }

        // Called on all clients
        [ClientRpc]
        public void RpcEquipWeapon()
        {
            if (equippedWeapon == null)
            {
                weaponDisplay.sprite = null;
                controller.projectileMount.localPosition = Vector3.zero;
                controller.fireRate = defaultFireRate;
                controller.meleeRange = defaultMeleeRange;
            }
            else
            {
                weaponDisplay.sprite = equippedWeapon.sprite;
                controller.projectileMount.localPosition = equippedWeapon.firePoint ? equippedWeapon.firePoint.position : Vector3.zero;
                controller.fireRate = equippedWeapon.fireRate;
                controller.meleeRange = equippedWeapon.meleeRange;
            }
        }

        private void Start()
        {
            if (hasAuthority) CmdEquipWeapon();
        }
    }
}