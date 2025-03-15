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

        public void healthEventOnHit(float damage)
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onHit(damage);
            }
        }

        public void healthEventOnDeath()
        {
            foreach (var _healthEvent in healthEventListeners)
            {
                _healthEvent.onDeath();
            }
        }
    }
}