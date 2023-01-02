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

        [SerializeField] private SpriteRenderer maskRenderer;
        [SerializeField] private Sprite[] masks;

        private void OnValidate()
        {
            if (healthManager == null)
                healthManager = GetComponent<HealthManager>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            healthManager.ModifyHealth(healthManager.StartHealth);

            InvokeRepeating(nameof(ServerUpdate), 0, 0.15f);
        }

        [ClientRpc]
        public void RpcSetMask(int i)
        {
            maskRenderer.sprite = masks[i];
        }

        [ServerCallback]
        public void ServerUpdate()
        {
            if (healthManager.isDead)
            {
                // Die
                animator.RpcSetTrigger("Died");

                RpcDisable();
            }
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