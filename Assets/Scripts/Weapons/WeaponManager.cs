using UnityEngine;
using Mirror;
using System.Collections;

namespace TopDownShooter
{
    public class WeaponManager : NetworkBehaviour
    {
        public CombatController controller;

        public SpriteRenderer weaponDisplay;

        public WeaponDictionary weaponSet;

        [Range(1, 100)]
        public float tossForce = 10;

        [SyncVar]
        public int currentWeapon;

        public WeaponObject equippedWeapon;

        public float defaultMeleeRange = 1;
        public float defaultFireRate = 0.5f;

        private iInput input;

        [SyncVar]
        public bool altFire;

        [ServerCallback]
        public void EquipWeapon(WeaponPickup pickup)
        {
            StartCoroutine(TryPickupWeapon(pickup));
        }

        [ServerCallback]
        private IEnumerator TryPickupWeapon(WeaponPickup pickup)
        {
            float pickupRange = 1f;
            var pos = pickup.transform.position;
            var _trans = transform;
            altFire = false;

            while (true)
            {
                yield return null;

                var dist = Vector2.Distance(_trans.position, pos);

                if (dist > pickupRange) break;

                if (altFire)
                {
                    yield return OnPickupWeapon(pickup.weapon);

                    NetworkServer.Destroy(pickup.gameObject);
                    break;
                }
            }

            altFire = false;
        }

        [ServerCallback]
        private IEnumerator OnPickupWeapon(WeaponObject weapon)
        {
            CmdDropWeapon();

            Debug.Log("Picking up");

            while (currentWeapon != -1) yield return new WaitForEndOfFrame();

            currentWeapon = weaponSet.GetWeaponIndex(weapon);

            CmdEquipWeapon();
        }

        // Called on server
        [Command]
        public void CmdEquipWeapon()
        {
            equippedWeapon = weaponSet.GetWeapon(currentWeapon);
            RpcEquipWeapon();
        }

        [Command]
        public void CmdDropWeapon()
        {
            RpcDropWeapon();
        }

        [Command]
        public void CmdTossWeapon()
        {
            RpcTossWeapon();
        }

        [ClientRpc]
        public void RpcTossWeapon()
        {
            if (currentWeapon >= 0)
            {
                // Drop the current weapon on the ground
                var instance = Instantiate(equippedWeapon.drop.gameObject, transform.position + transform.right, transform.rotation);
                instance.GetComponent<Rigidbody2D>().velocity = transform.right * tossForce;
                NetworkServer.Spawn(instance);

                currentWeapon = -1;

                CmdEquipWeapon();
            }
        }

        [ClientRpc]
        public void RpcDropWeapon()
        {
            if (currentWeapon >= 0)
            {
                // Drop the current weapon on the ground
                var instance = Instantiate(equippedWeapon.drop.gameObject, transform.position - transform.up, transform.rotation);
                NetworkServer.Spawn(instance);

                currentWeapon = -1;

                CmdEquipWeapon();
            }
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
            if (hasAuthority)
            {
                CmdEquipWeapon();
                input = GetComponent<iInput>();
                input.altFireEnded += () => altFire = true;
            }
        }

        private void Update()
        {
            if (hasAuthority == false) return;

            if (altFire && currentWeapon != -1)
            {
                // Toss weapon

                CmdTossWeapon();
            }
        }
    }
}