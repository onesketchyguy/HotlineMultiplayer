using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TopDownShooter
{
    public class CharacterAnimator : NetworkBehaviour
    {
        public Animator anim;

        [Command]
        public void CmdSetTrigger(string name)
        {
            RpcSetTrigger(name);
        }

        [ClientRpc]
        private void RpcSetTrigger(string name)
        {
            anim.SetTrigger(name);
        }
    }
}