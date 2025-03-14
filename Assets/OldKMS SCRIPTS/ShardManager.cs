using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class ShardManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider shardSlider;
    [SerializeField] private float maxShards = 10;
    [SerializeField] private float currentShards = 0;
    [SerializeField] private List<Upgrade> upgrades = new List<Upgrade>();
    [SerializeField] private RawImage upgrade1;
    [SerializeField] private RawImage upgrade2;
    [SerializeField] private Sprite upgrade1Sprite;
    [SerializeField] private Sprite upgrade2Sprite;
    [SerializeField] private Sprite upgrade3Sprite;

    [SerializeField]
    private TextMeshProUGUI shardText;
    private bool upgradeAvailable = false;
    
    private Upgrade[] currentUpgrades = new Upgrade[2];
    private Player plr;
    
    void Start()
    {
        this.plr = GetComponent<Player>();
        upgrades.Add(new Upgrade("Upgrade1", 5, upgrade1Sprite));
        upgrades.Add(new DmgUpgrade("Dmg Upgrade", 7, plr, upgrade2Sprite));
        upgrades.Add(new SpeedUpgrade("Speed Upgrade", 10, plr, upgrade3Sprite));
    }

    // Update is called once per frame
    void Update()
    {
        if (upgradeAvailable)
        {
            upgrade1.gameObject.SetActive(true);
            upgrade2.gameObject.SetActive(true);
            upgrade1.texture = currentUpgrades[0].getTexture();
            upgrade2.texture = currentUpgrades[1].getTexture();
        }
        else
        {
            upgrade1.gameObject.SetActive(false);
            upgrade2.gameObject.SetActive(false);
        }
        shardSlider.value = currentShards;
        shardSlider.maxValue = maxShards;
        shardText.text = currentShards + " / " + maxShards;
        // collision
        if (currentShards >= 5 && !upgradeAvailable)
        {
            upgradeAvailable = true;
            currentUpgrades[0] = genUpgrade(null);
            currentUpgrades[1] = genUpgrade(currentUpgrades[0]);
        }
    }
    
    public void addShard()
    {
        currentShards++;
        currentShards = Mathf.Clamp(currentShards, 0, maxShards);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Shard"))
        {
            Destroy(other.gameObject);
            addShard();
        }
    }

    private void OnJedn(InputValue value)
    {
        Debug.Log("jedn");
        if (!upgradeAvailable) return;
        currentUpgrades[0].dodojNagrode();
        this.reset();
    }

    private void OnDwo(InputValue value)
    {
        Debug.Log("dwo");
        if (!upgradeAvailable) return;
        currentUpgrades[1].dodojNagrode();
        this.reset();
    }

    private void reset()
    {
        currentUpgrades = new Upgrade[2];
        upgradeAvailable = false;
    }
    private Upgrade genUpgrade(Upgrade lastUpgrade)
    {
        Upgrade generatedUpgrade = upgrades[UnityEngine.Random.Range(0, upgrades.Count)];
        if (generatedUpgrade == lastUpgrade)
        {
            return genUpgrade(lastUpgrade);
        }
        return generatedUpgrade;
    }
}
