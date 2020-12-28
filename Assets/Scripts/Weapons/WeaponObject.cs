using UnityEngine;

namespace TopDownShooter
{
    public class WeaponObject : MonoBehaviour
    {
        public SpriteRenderer spriterenderer;

        public Sprite sprite
        {
            get
            {
                return spriterenderer.sprite;
            }
        }

        public Transform firePoint;

        [Range(0.1f, 1.5f)]
        public float fireRate = 0.1f;
    }
}