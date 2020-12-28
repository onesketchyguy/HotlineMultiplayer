using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class CameraPeg : NetworkBehaviour
    {
        private HealthManager playerHealth;
        private Transform player;

        private bool playerDead
        {
            get
            {
                return (playerHealth != null && playerHealth.isDead == true);
            }
        }

        private bool playerActive
        {
            get
            {
                return player != null && player.gameObject.activeSelf;
            }
        }

        private Transform _transform;

        private PlayerInput[] playerList;

        public Transform FindPlayer()
        {
            playerList = FindObjectsOfType<PlayerInput>();

            foreach (var item in playerList)
            {
                if (item.hasAuthority)
                {
                    playerHealth = item.GetComponent<HealthManager>();
                    if (playerDead)
                    {
                        playerHealth = null;

                        Debug.Log($"Skipping {item.name}");

                        continue;
                    }

                    return item.transform;
                }
            }

            return null;
        }

        private void Start()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            if (playerActive == false || playerDead == true)
            {
                if (Time.frameCount % 3 == 0)
                    player = FindPlayer();
                return;
            }

            _transform.position = player.position;
        }
    }
}