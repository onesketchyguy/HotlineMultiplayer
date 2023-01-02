using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class CombatController : NetworkBehaviour
    {
        public iInput input;

        public string playerLayer = "Player";

        public GameObject projectilePrefab;

        public Transform projectileMount;

        public AudioClip attClip = null;
        public PhysicalObject.ObjectType attType = PhysicalObject.ObjectType.Blunt;

        [HideInInspector]
        public float fireRate = .5f;

        [HideInInspector]
        public float meleeRange = 1;

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

            if (attClip != null)
            {
                AudioSource.PlayClipAtPoint(attClip, projectileMount.position);
            }

            if (meleeRange == 0)
            {
                GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
                projectile.layer = LayerMask.NameToLayer(playerLayer);
                NetworkServer.Spawn(projectile);
            }
            else
            {
                // Do melee
                var hits = Physics2D.OverlapBoxAll(projectileMount.position, Vector2.one * meleeRange, projectileMount.rotation.z);

                Debug.DrawLine(projectileMount.position, projectileMount.position + (projectileMount.right * meleeRange * 0.5f), Color.red, 1);

                foreach (var hit in hits)
                {
                    if (hit.gameObject == gameObject) continue;

                    var health = hit.GetComponent<HealthManager>();

                    if (health != null)
                    {
                        health.HandleHitEvent(attType);
                        health.ModifyHealth(-100);
                    }
                }
            }
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