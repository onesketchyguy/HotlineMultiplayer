using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class GameStateManager : MonoBehaviour
    {
        public bool allEnemiesDead = false;

        private void Update()
        {
            var enemies = FindObjectsOfType<AIInput>();
            if (enemies == null) allEnemiesDead = true;
            else
            {
                allEnemiesDead = true;

                foreach (var item in enemies)
                {
                    if (item.isActiveAndEnabled)
                    {
                        allEnemiesDead = false;
                    }
                }
            }

            var players = FindObjectsOfType<PlayerInput>();
            if (players == null) Debug.Log("Failure!");
        }
    }
}
