using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class AIInput : NetworkBehaviour, iInput
    {
        public Vector2 lookDirection { get; set; }
        public Vector2 moveDirection { get; set; }
        public OnFire fireStart { get; set; }
        public OnFire altFireStart { get; set; }
        public OnFire fireEnded { get; set; }
        public OnFire altFireEnded { get; set; }

        public OnFire onExecution { get; set; }

        public Transform[] wanderPath;

        private Transform target;

        // AI NEEDS TO DO THINGS

        [ServerCallback]
        public override void OnStartServer()
        {
            base.OnStartServer();
            InvokeRepeating(nameof(ServerUpdate), 0, 0.1f);
        }

        void SetTarget() 
        {
            var player = FindObjectOfType<PlayerInput>();

            if (player == null) return;

            target = player.transform;
        }

        [ServerCallback]
        void ServerUpdate()
        {
            SetTarget();

            if (target != null)
            {
                lookDirection = target.position;

                Shoot();
            }
        }

        void Shoot()
        {
            CancelInvoke(nameof(StopShooting));
            fireStart?.Invoke();
            Invoke(nameof(StopShooting), 0.1f);
        }

        void StopShooting() => fireEnded?.Invoke();
    }
}