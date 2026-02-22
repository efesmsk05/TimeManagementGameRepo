using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public int levelIndex;
    public string levelName = "LEVEL ??";
    

    [Header("Upgrade Requirements")]
    public float costToUpgrade ; // fiyat
    public float requiredReputation; // İtibar Exp

    [Header("Constraints")] // Kısıtlamalar

    public int maxCustomerCap;
    public int maxTableCap;

    [Header("Days")] 
    public List<DayEntry> dayData; // Gün veri listesi
    
    
}

[System.Serializable]
public struct DayEntry
{
    public DayDataSO dayData;
    [Range(1, 100)] public int weight; // Gelme ihtimali ağırlığı (Örn: Normal=80, Zor=20)
}

    