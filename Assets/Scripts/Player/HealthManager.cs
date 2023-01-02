using Mirror;
using UnityEngine;

namespace TopDownShooter
{
    public class HealthManager : NetworkBehaviour
    {
        public delegate void OnValueChanged(float mod);

        public OnValueChanged onHealthChanged;

        public float StartHealth = 100;

        [SyncVar(hook = nameof(OnHealthModified))]
        public float currentHealth;

        [SyncVar]
        public bool isDead;

        [System.Serializable]
        private struct HitSound
        {
            public PhysicalObject.ObjectType hitType;
            public AudioClip clip;
        }

        [SerializeField] private HitSound[] hitSounds = null;

        public void ModifyHealth(float mod)
        {
            isDead = currentHealth + mod <= 0;
            currentHealth += mod;
        }

        public void OnHealthModified(float lastHealth, float currentHealth)
        {
            // We should probably do something here...
            // ¯\_(ツ)_/¯

            onHealthChanged?.Invoke(lastHealth - currentHealth);
        }

        internal void HandleHitEvent(PhysicalObject.ObjectType type)
        {
            // Play sound
            foreach (var item in hitSounds)
            {
                if (item.hitType == type)
                {
                    AudioSource.PlayClipAtPoint(item.clip, transform.position);

                    break;
                }
            }

            // Handle effect
            switch (type)
            {
                //Won't do damage or knock over the target.
                case PhysicalObject.ObjectType.Light:
                    // FIXME: React
                    break;
                //Will knock over the target.
                case PhysicalObject.ObjectType.Blunt:
                    // FIXME: Fall unconscious
                    break;
                //Has a high chance of killing the target.
                case PhysicalObject.ObjectType.Sharp:
                    // FIXME: Bleed
                    break;
                default:
                    break;
            }
        }
    }
}