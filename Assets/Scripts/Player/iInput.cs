using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public delegate void OnFire();

    public interface iInput
    {
        public Vector2 lookDirection { get; set; }
        public Vector2 moveDirection { get; set; }

        public OnFire fireStart { get; set; }
        public OnFire altFireStart { get; set; }

        public OnFire fireEnded { get; set; }
        public OnFire altFireEnded { get; set; }
    }
}