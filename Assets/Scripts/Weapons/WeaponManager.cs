using UnityEngine;
using Mirror;
using System.Collections;
using System.Reflection;

namespace TopDownShooter
{
    public class WeaponManager : NetworkBehaviour
    {
        public CombatController controller;

        public SpriteRenderer weaponDisplay;

        public WeaponDictionary weaponSet;

        [Range(1, 100)]
        public float tossForce = 10;

        public int currentWeapon;

        public WeaponObject equippedWeapon;

        public float defaultMeleeRange = 1;
        public float defaultFireRate = 0.5f;

        private iInput input;

        [SyncVar]
        public bool altFire;

        [ServerCallback]
        public void EquipWeapon(WeaponPickup weapon)
        {
            StartCoroutine(TryPickupWeapon(weapon));
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

                if (altFire || currentWeapon == -1)
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
            while (currentWeapon != -1)
            {
                CmdDropWeapon();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

            int index = weaponSet.GetWeaponIndex(weapon);
            Debug.Log($"Picking up {weapon.name} ({index})");
            CmdEquipWeapon(index);
        }

        // Called on server
        [Command]
        public void CmdEquipWeapon(int w) { RpcEquipWeapon(w); Debug.Log($"CMD transport weapon no.{w}"); }

        [Command]
        public void CmdDropWeapon() => RpcDropWeapon();

        [Command]
        public void CmdTossWeapon() => RpcTossWeapon();

        [ClientRpc]
        public void RpcTossWeapon()
        {
            if (currentWeapon >= 0)
            {
                currentWeapon = -1;

                // Throw the current weapon on the ground
                var instance = Instantiate(equippedWeapon.drop.gameObject, transform.position + transform.right, transform.rotation);
                instance.GetComponent<Rigidbody2D>().velocity = transform.right * tossForce;
                NetworkServer.Spawn(instance);

                CmdEquipWeapon(-1);
            }
        }

        [ClientRpc]
        public void RpcDropWeapon()
        {
            if (currentWeapon >= 0)
            {
                Debug.Log($"Drop weapon {weaponSet.GetWeapon(currentWeapon).name} ({currentWeapon})");

                currentWeapon = -1;

                // Drop the current weapon on the ground
                var instance = Instantiate(equippedWeapon.drop.gameObject, transform.position - transform.up, transform.rotation);
                NetworkServer.Spawn(instance);

                CmdEquipWeapon(-1);
            }
        }

        // Called on all clients
        [ClientRpc]
        public void RpcEquipWeapon(int w)
        {
            currentWeapon = w;
            equippedWeapon = weaponSet.GetWeapon(currentWeapon);

            Debug.Log($"Trying to equip index {w}");

            if (equippedWeapon == null)
            {
                weaponDisplay.sprite = null;
                controller.projectileMount.localPosition = Vector3.zero;
                controller.fireRate = defaultFireRate;
                controller.meleeRange = defaultMeleeRange;
                controller.attClip = null;
                controller.attType = PhysicalObject.ObjectType.Blunt;

                Debug.Log("Equipping null weapon");
            }
            else
            {
                weaponDisplay.sprite = equippedWeapon.sprite;
                controller.projectileMount.localPosition = equippedWeapon.firePoint ? equippedWeapon.firePoint.position : Vector3.zero;
                controller.fireRate = equippedWeapon.fireRate;
                controller.meleeRange = equippedWeapon.meleeRange;
                controller.attClip = equippedWeapon.attClip;
                controller.attType = equippedWeapon.attType;

                Debug.Log($"Equipping {equippedWeapon.name}({w})");
            }
        }

        private void Start()
        {
            if (hasAuthority)
            {
                CmdEquipWeapon(currentWeapon);
                input = GetComponent<iInput>();
                input.altFireStart += () => altFire = true;
                input.altFireEnded += () => altFire = false;
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