using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class PhysicalObject : MonoBehaviour
    {
        public enum ObjectType
        {
            [Tooltip("Won't do damage or knock over the target.")]
            Light,

            [Tooltip("Will knock over the target.")]
            Blunt,

            [Tooltip("Has a high chance of killing the target.")]
            Sharp
        }

        public ObjectType type;

        private Rigidbody2D rigidBody;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (rigidBody.velocity.magnitude <= 0.2f) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                var health = collision.gameObject.GetComponent<HealthManager>();

                if (health) health.HandleHitEvent(type);
            }
        }
    }
}