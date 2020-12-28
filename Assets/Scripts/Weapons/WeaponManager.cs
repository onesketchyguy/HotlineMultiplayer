using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class WeaponManager : NetworkBehaviour
    {
        public CombatController controller;

        public SpriteRenderer weaponDisplay;

        public WeaponObject equippedWeapon;

        // Called on server
        [Command]
        public void CmdEquipWeapon()
        {
            RpcEquipWeapon();
        }

        // Called on all clients
        [ClientRpc]
        public void RpcEquipWeapon()
        {
            weaponDisplay.sprite = equippedWeapon.sprite;
            controller.projectileMount.localPosition = equippedWeapon.firePoint.position;
            controller.fireRate = equippedWeapon.fireRate;
        }

        private void Start()
        {
            if (isLocalPlayer)
                CmdEquipWeapon();
        }
    }
}