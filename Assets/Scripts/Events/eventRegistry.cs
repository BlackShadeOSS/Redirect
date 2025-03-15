using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class eventRegistry : MonoBehaviour
    {
        public static List<healthEvent> healthEventListeners = new List<healthEvent>();

        public static void addHealthEvent(healthEvent e)
        {
            healthEventListeners.Add(e);
            Debug.Log("Registered new health event, count: " + healthEventListeners.Count);
        }

        public static void removeHealthEvent(healthEvent e)
        {
            healthEventListeners.Remove(e);
        }

        public static void healthEventOnHit(GameObject player, float damage)
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onHit(player, damage);
            }
        }

        public static void healthEventOnDeath(GameObject player)
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onDeath(player);
            }
        }
    }
}