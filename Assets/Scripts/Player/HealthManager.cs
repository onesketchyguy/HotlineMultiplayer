using Mirror;

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
            //throw new System.NotImplementedException();
        }
    }
}