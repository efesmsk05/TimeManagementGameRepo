using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    KitchenSpeed,
    PlayerSpeed,
    StorageCapacity
}



[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/Upgrade Data")]
public class UpgradeDataSO : ScriptableObject
{
    public UpgradeType upgradeType;
    public string upgradeName;
    public Sprite icon;

    [Header ("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 5;

    [Header("Cost Settings")]
    public float baseCost = 100f;  
    public float costMultiplier = 1.5f; // her levelde maaliyeti yüzde 50 artır

    [Header("Value Settings")]
    public float baseValuse = 1f;
    public float valueMultiplier = 1.2f; // her levelde değeri yüzde 20 artır


    public float GetCurrentCost()
    {
        return baseCost * Mathf.Pow(costMultiplier, currentLevel-1);// Current level 0 oldığı için baseCost döner
    }

    public float GetCurrentValue()
    {
        return baseValuse * Mathf.Pow(valueMultiplier, currentLevel-1);
    }

    public bool CanLevelUp()
    {
        return currentLevel < maxLevel;
    }

    public void LevelUp()
    {
        if (CanLevelUp())
        {
            currentLevel++;
        }
    }

}   
