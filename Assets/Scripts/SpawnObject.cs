using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class SpawnObject : NetworkBehaviour
    {
        public NetworkBehaviour _object;

        public override void OnStartServer()
        {
            base.OnStartServer();

            var point = transform;

            var instance = Instantiate(_object.gameObject, point.position, point.rotation);
            NetworkServer.Spawn(instance.gameObject, connectionToServer);
        }
    }
}