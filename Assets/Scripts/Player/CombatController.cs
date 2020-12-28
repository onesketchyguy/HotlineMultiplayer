using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class CombatController : NetworkBehaviour
    {
        public iInput input;

        public float fireRate = .5f;

        public GameObject projectilePrefab;

        public Transform projectileMount;

        public OnFire fireStart { get; set; }
        public OnFire altFireStart { get; set; }
        public OnFire fireEnded { get; set; }
        public OnFire altFireEnded { get; set; }

        private void Initialize()
        {
            input = GetComponent<iInput>();

            fireStart = () =>
            {
                InvokeRepeating(nameof(CmdOnPrimaryFire), 0, fireRate);
            };

            fireEnded += () =>
            {
                CancelInvoke(nameof(CmdOnPrimaryFire));
                CmdOnPrimaryFireStopped();
            };

            altFireStart += () =>
            {
                CmdOnSecondaryFire();
            };

            altFireEnded += () =>
            {
                CmdOnSecondaryFireStopped();
            };
        }

        private void OnEnable()
        {
            Initialize();

            input.fireStart += fireStart;
            input.fireEnded += fireEnded;
            input.altFireStart += altFireStart;
            input.altFireEnded += altFireEnded;
        }

        private void OnDisable()
        {
            input.fireStart -= fireStart;
            input.fireEnded -= fireEnded;
            input.altFireStart -= altFireStart;
            input.altFireEnded -= altFireEnded;
        }

        [Command]
        public void CmdOnPrimaryFire()
        {
            if (enabled == false) return;

            GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        [Command]
        public void CmdOnPrimaryFireStopped()
        {
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        private void RpcOnFire()
        {
            //animator.SetTrigger("Shoot");
        }

        [Command]
        public void CmdOnSecondaryFire()
        {
            if (enabled == false) return;
        }

        [Command]
        public void CmdOnSecondaryFireStopped()
        {
        }
    }
}