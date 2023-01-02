using UnityEngine;
using Mirror;
//using static UnityEditor.LightingExplorerTableColumn;

namespace TopDownShooter
{
    public class Projectile : NetworkBehaviour
    {
        public float destroyAfter = 5;
        public Rigidbody2D rigidBody;
        public float force = 1000;
        public PhysicalObject.ObjectType attType = PhysicalObject.ObjectType.Sharp;

        public float damage = 100;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        private void OnEnable()
        {
            rigidBody.AddForce(transform.right * force);
        }

        // destroy for everyone on the server
        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        // ServerCallback because we don't want a warning if OnTriggerEnter is
        // called on the client
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D co)
        {
            var health = co.GetComponent<HealthManager>();

            if (health)
            {
                health.ModifyHealth(-damage);
                health.HandleHitEvent(attType);
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}