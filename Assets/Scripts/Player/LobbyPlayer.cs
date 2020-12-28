using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mirror;

namespace TopDownShooter
{
    public class LobbyPlayer : NetworkBehaviour
    {
        [Header("UI setup")]
        public string ReadyToggleChildName = "ReadyToggle";

        public string ReadyPlayersRegion = "ReadyPlayersRegion";

        [SyncVar]
        public bool isReady;

        private bool allReady
        {
            get
            {
                foreach (var item in FindObjectsOfType<LobbyPlayer>())
                    if (item.isReady == false) return false;

                return true;
            }
        }

        private int playingAs = 0;

        private bool spawning = false;

        public GameObject[] playerPrefabs;

        [SyncVar]
        public HealthManager player;

        private NetworkStartPosition[] spawnPoints;

        public GameObject LobbyPrefab;
        private GameObject lobby;

        public GameObject readyCheckMark;

        private Transform readyPlayersRegion;
        private Transform[] readyCheckMarks;

        private LobbyPlayer[] players;

        private void Start()
        {
            if (hasAuthority == false) return;

            // Open some menus if the game is not in progress
            lobby = Instantiate(LobbyPrefab, transform);

            // Show the active and inactive players
            foreach (var item in lobby.GetComponentsInChildren<Transform>())
            {
                if (item.name == ReadyToggleChildName)
                {
                    // Add listener
                    item.GetComponent<Toggle>().onValueChanged.AddListener(CmdSetReady);
                }
                else if (item.name == ReadyPlayersRegion)
                {
                    readyPlayersRegion = item.transform;
                }
            }

            lobby.SetActive(true);
        }

        private void Update()
        {
            if (allReady == false || player != null)
            {
                players = FindObjectsOfType<LobbyPlayer>();

                if (allReady == false) UpdateReadyObjects();
            }
            else
            {
                if (spawning == false)
                {
                    spawning = true;
                    StartCoroutine(SpawnPlayer());
                }
            }
        }

        private void UpdateReadyObjects()
        {
            // Make sure that the active objects are the correct length then update them
            if (readyCheckMarks == null || readyCheckMarks.Length < players.Length - 1)
            {
                // Setup check marks
                readyCheckMarks = new Transform[players.Length - 1];

                for (int i = 0; i < readyCheckMarks.Length; i++)
                {
                    var obj = Instantiate(readyCheckMark, readyPlayersRegion);
                    foreach (var item in obj.GetComponentsInChildren<Transform>())
                    {
                        if (item.name == "Checkmark")
                        {
                            readyCheckMarks[i] = item;
                            break;
                        }
                    }
                }

                return;
            }

            // Update check marks
            int p = 0;
            foreach (var item in players)
            {
                if (item == this) continue;

                readyCheckMarks[p].gameObject.SetActive(item.isReady);

                p++;
            }
        }

        private IEnumerator SpawnPlayer()
        {
            // Spawn the player
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            CmdSpawnPlayer(Random.Range(0, spawnPoints.Length));

            while (player == null)
            {
                foreach (var item in FindObjectsOfType<PlayerInput>())
                    if (item.hasAuthority)
                        player = item.GetComponent<HealthManager>();
                yield return new WaitForEndOfFrame();
            }

            player.onHealthChanged += _ =>
            {
                if (player.isDead)
                {
                    // Become active
                    transform.position = player.transform.position;

                    gameObject.SetActive(true);
                }
            };

            // Hide self
            //lobby.SetActive(false);
            gameObject.SetActive(false);
        }

        [Command]
        public void CmdSetReady(bool value)
        {
            isReady = value;
        }

        [Command]
        public void CmdSpawnPlayer(int index)
        {
            var point = spawnPoints[index].transform;

            player = Instantiate(playerPrefabs[playingAs], point.position, point.rotation).GetComponent<HealthManager>();
            NetworkServer.Spawn(player.gameObject, connectionToClient);
        }
    }
}