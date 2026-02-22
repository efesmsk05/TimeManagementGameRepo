using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDayData", menuName = "ScriptableObjects/DayDataSO")]
public class DayDataSO : ScriptableObject
{
    public string dayName = "DAY ??";
    [TextArea ] public string dayDescription = "Açıklama yok.";

    [Header("Day Multiplier Settings")]
    public float goalMultiplier = 1f;
    public float spawnRateMultiplier = 1f;

    [Header("Day Time Settings")]
    public int dayStartTime = 8; // 8 AM
    public int dayEndTime = 20; // 8 PM


    // [Header("Day Specific Events")]
   // public List<string> sideQuests;

    //special rules
    
    // yan görevler
   // public List<string> specialEvents; // O gün için özel etkinlikler

}

