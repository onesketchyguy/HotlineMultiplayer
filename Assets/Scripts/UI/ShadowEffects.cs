using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class ShadowEffects : MonoBehaviour
    {
        public UnityEngine.UI.Shadow shadowComponent;
        public Vector2 maxExtent;
        public float randomness = 10.0f;

        float t = 0;

        private void Start()
        {
            t = Random.value * randomness;
        }

        private void Update()
        {
            t += Time.deltaTime;

            var v = shadowComponent.effectDistance;
            v.x = Mathf.Sin(t) * maxExtent.x;
            v.y = Mathf.Cos(t) * maxExtent.y;

            shadowComponent.effectDistance = v;
        }
    }
}
