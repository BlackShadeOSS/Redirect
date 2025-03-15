using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class eventRegistry : MonoBehaviour
    {
        [SerializeField] private List<healthEvent> healthEventListeners = new List<healthEvent>();

        public void addHealthEvent(healthEvent e)
        {
            healthEventListeners.Add(e);
        }

        public void removeHealthEvent(healthEvent e)
        {
            healthEventListeners.Remove(e);
        }

        public void healthEventOnHit(GameObject player, float damage)
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onHit(player, damage);
            }
        }

        public void healthEventOnDeath(GameObject player)
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onDeath(player);
            }
        }
    }
}