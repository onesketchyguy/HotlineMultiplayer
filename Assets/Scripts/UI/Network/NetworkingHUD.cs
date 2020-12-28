using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class NetworkingHUD : MonoBehaviour
    {
        public NetworkManager networkManager
        {
            get
            {
                if (_networkManager == null) _networkManager = FindObjectOfType<NetworkManager>();

                return _networkManager;
            }
        }

        private NetworkManager _networkManager;

        public void JoinLocalGame() => networkManager.StartClient();

        public void StopClient()
        {
            networkManager.StopClient();

            // We need to be able to tell if this is also the host and end that aswell
        }

        public void HostGame() => networkManager.StartHost();

        public void StopHost() => networkManager.StopHost();
    }
}