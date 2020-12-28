using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    [RequireComponent(typeof(HealthManager))]
    public class CharacterManager : NetworkBehaviour
    {
        public HealthManager healthManager;

        public MonoBehaviour[] toDisableOnDeath;

        public Collider2D colliderToDisable;

        public CharacterAnimator animator;

        private void OnValidate()
        {
            if (healthManager == null)
                healthManager = GetComponent<HealthManager>();
        }

        private void Start()
        {
            if (hasAuthority == false) return;

            healthManager.ModifyHealth(healthManager.StartHealth);

            healthManager.onHealthChanged += (float mod) =>
            {
                if (healthManager.isDead)
                {
                    // Die
                    animator.CmdSetTrigger("Died");

                    CmdDisable();
                }

                if (mod > 0)
                {
                    // Got health
                }
                else
                {
                    // Was hurt
                }
            };
        }

        [Command]
        public void CmdDisable()
        {
            RpcDisable();
        }

        [ClientRpc]
        public void RpcDisable()
        {
            for (int i = 0; i < toDisableOnDeath.Length; i++)
                toDisableOnDeath[i].enabled = false;

            colliderToDisable.enabled = false;
        }
    }
}