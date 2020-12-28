using UnityEngine;

namespace TopDownShooter
{
    public class MovementController : MonoBehaviour
    {
        public iInput input;

        public Rigidbody2D rigidBody;

        private Transform _transform;

        private float degreeConversion;

        private Vector2 lookDirection;

        [Range(1, 10f)]
        public float speed = 8.7f;

        private void Start()
        {
            degreeConversion = (180 / Mathf.PI);

            _transform = transform;
            input = GetComponent<iInput>();
        }

        private void FixedUpdate()
        {
            if (!enabled) return;

            // Move
            rigidBody.velocity = input.moveDirection * speed;
            rigidBody.angularVelocity = 0;

            // Look
            if (lookDirection != input.lookDirection)
            {
                lookDirection = input.lookDirection;

                float AngleRad = Mathf.Atan2(lookDirection.y - _transform.position.y, lookDirection.x - _transform.position.x);
                float AngleDeg = degreeConversion * AngleRad;
                _transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
            }
        }
    }
}