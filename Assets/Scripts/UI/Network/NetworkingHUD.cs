using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mirror;
using Mirror.Discovery;

namespace TopDownShooter
{
    public class NetworkingHUD : MonoBehaviour
    {
        public NetworkDiscovery networkDiscovery
        {
            get
            {
                if (_networkDiscovery == null) _networkDiscovery = FindObjectOfType<NetworkDiscovery>();

                return _networkDiscovery;
            }
        }
        public NetworkManager networkManager
        {
            get 
            {
                if (_networkManager == null) _networkManager = FindObjectOfType<NetworkManager>();
                return _networkManager;
            }
        }

        private NetworkManager _networkManager;
        private NetworkDiscovery _networkDiscovery;

        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        Vector2 scrollViewPos = Vector2.zero;

        public GameObject joinButtonPrefab;
        private List<GameObject> serverListObjects = new List<GameObject>();

        public void Start() 
        {
            networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        }

        public void Update() 
        {
            if (Time.frameCount % 30 == 0) 
                UpdateServerList();
        }

        public void StopHost() => networkManager.StopHost();

        public void JoinLocalGame() => networkManager.StartClient();

        public void StopClient()
        {
            networkManager.StopClient();

            // We need to be able to tell if this is also the host and end that aswell
        }

        /// Find Servers
        public void UpdateServerList() 
        {
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();

            Invoke(nameof(UpdateObjectList), 0.5f);
        }

        private void UpdateObjectList() 
        {
            // Clear the existing list
            foreach (var item in serverListObjects) {
                Destroy(item.gameObject);
            }

            serverListObjects.Clear();

            foreach (ServerResponse info in discoveredServers.Values) 
            {
                var obj = Instantiate(joinButtonPrefab, transform);
                var button = obj.GetComponent<UnityEngine.UI.Button>();

                button.onClick.AddListener(()=>Connect(info));
            }
        }

        /// Host a local game
        public void StartHost() 
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
        }

        /// Start a dedicated server
        public void StartServer() 
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();

            networkDiscovery.AdvertiseServer();
        }

        void Connect(ServerResponse info)
        {
            NetworkManager.singleton.StartClient(info.uri);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
        }
    }
}