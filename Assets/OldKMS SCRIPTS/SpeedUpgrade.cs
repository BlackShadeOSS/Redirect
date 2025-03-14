using UnityEngine;

namespace DefaultNamespace
{
    public class SpeedUpgrade : Upgrade
    {
        private Player plr;
        public SpeedUpgrade(string name, int koszt, Player player, Sprite sprite) : base(name, koszt, sprite)
        {
            plr = player;
        }
        public void dodojNagrode()
        {
            Debug.Log("Speed upgrade");
            plr.addSpeed(1);
        }
    }
}