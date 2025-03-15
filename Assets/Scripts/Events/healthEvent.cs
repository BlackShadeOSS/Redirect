using UnityEngine;

namespace Events
{
    public interface healthEvent
    {
        void onDeath(GameObject player);
        void onHit(GameObject player, float damage);
    }
}