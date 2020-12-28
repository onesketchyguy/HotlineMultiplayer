using UnityEngine;

namespace TopDownShooter
{
    public class ScaleTween : MonoBehaviour
    {
        public Vector3 scaleTo;

        public LeanTweenType curveType;
        public AnimationCurve curve;

        public float time = 1;

        // Start is called before the first frame update
        private void Start()
        {
            if (curveType == LeanTweenType.animationCurve)
                LeanTween.scale(gameObject, scaleTo, time).setEase(curve).setLoopPingPong();
            else LeanTween.scale(gameObject, scaleTo, time).setEase(curveType).setLoopPingPong();
        }
    }
}