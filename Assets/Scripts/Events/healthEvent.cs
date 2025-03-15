namespace Events
{
    public interface healthEvent
    {
        void onDeath();
        void onHit(float damage);
    }
}