using UnityEngine;

namespace DefaultNamespace
{
    public class DmgUpgrade : Upgrade
    {
        Player player;
        public DmgUpgrade(string name, int koszt, Player player, Sprite sprite) : base(name, koszt, sprite)
        {
            this.player = player;
        }
        
        public void dodajNagrode()
        {
            player.addDmg(5);
        }
    }
}