using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    private string Name;
    private int Koszt;
    private Sprite sprite;

    public Upgrade(string name, int koszt, Sprite sprite)
    {
        Name = name;
        Koszt = koszt;
        this.sprite = sprite;
    }

    public void dodojNagrode()
    {
        
    }

    public int getKoszt()
    {
        return this.Koszt;
    }
    public Texture getTexture()
    {
        return this.sprite.texture;
    }
}
