using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class MoveTween : MonoBehaviour
    {
        public Vector3 moveTo;

        public LeanTweenType curveType;
        public AnimationCurve curve;

        public float time = 1;

        // Start is called before the first frame update
        private void Start()
        {
            if (curveType == LeanTweenType.animationCurve)
                LeanTween.move(gameObject, transform.position + moveTo, time).setEase(curve).setLoopPingPong();
            else LeanTween.move(gameObject, transform.position + moveTo, time).setEase(curveType).setLoopPingPong();
        }
    }
}