using Mirror;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : NetworkBehaviour
    {
        protected virtual void OnPlayerPickup(GameObject sender)
        {
            Debug.Log($"{gameObject.name} being picked up by {sender.name}...");
            // Do things
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // Tell the player to pickup this item
                OnPlayerPickup(collision.gameObject);
            }
        }
    }
}