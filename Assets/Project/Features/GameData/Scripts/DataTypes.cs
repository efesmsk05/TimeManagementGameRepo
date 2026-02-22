using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tüm veriler bu çatının altında toplanır.
[System.Serializable]
public class SaveData
{
    // Ekonomi ve Oyuncu İlerlemesi
    public PlayerData playerData = new PlayerData();

    // Dükkan İstatistikleri (Level, XP, İsim)
    public ShopStatsData shopStats = new ShopStatsData();

    // Dükkan Görünümü (Duvar rengi, Masa tipi vb.)
    public ShopDesignData shopDesign = new ShopDesignData();

    // Oyun Ayarları (Ses, Dil vb.)
    public SettingsData settings = new SettingsData();

    // Upgrade Listesi (Zaten SO tabanlı sisteminle harika çalışıyor)
    public List<UpgradeSaveData> upgrades = new List<UpgradeSaveData>();

    // Constructor: SaveData oluşturulduğunda alt sınıflar null kalmasın diye
    public SaveData()
    {
        playerData = new PlayerData();
        shopStats = new ShopStatsData();
        shopDesign = new ShopDesignData();
        settings = new SettingsData();
        upgrades = new List<UpgradeSaveData>();
    }
}

// --- 1. OYUNCU VE EKONOMİ VERİLERİ ---
[System.Serializable]
public class PlayerData
{
    public float totalMoney;
    public int currentDayIndex;
    public float totalPlayTime; // Oyuncunun oyunda geçirdiği toplam süre
}

// --- 2. DÜKKAN İSTATİSTİKLERİ ---
[System.Serializable]
public class ShopStatsData
{
    public string shopName;
    public int shopLevel;
    public float currentXp;

    public int currentLevelIndex;
    public float reputation; // Dükkan itibarı (Yıldız)
}

// --- 3. DÜKKAN TASARIM VERİLERİ (CUSTOMIZATION) ---
[System.Serializable]
public class ShopDesignData
{
    // Örnek: Duvar kağıdı ID'si, Zemin ID'si
    public int wallPatternID;
    public int floorPatternID;
    
    // Masa sandalye düzeni ileride buraya eklenebilir
    // public List<FurnitureData> furnitures;
}

// --- 4. AYARLAR ---
[System.Serializable]
public class SettingsData
{
    public float musicVolume;
    public float sfxVolume;
    public bool isVibrationOn;
    public int languageIndex; // 0: Eng, 1: Tr vb.
}

// --- 5. UPGRADE YARDIMCISI ---
[System.Serializable]
public class UpgradeSaveData
{
    public UpgradeType type;
    public int level;
}