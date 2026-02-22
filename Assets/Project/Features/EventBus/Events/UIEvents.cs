

using UnityEngine;

public static class UIEvent
{   
    
    public struct ComboUpdateEvent
    {
        public int currentCombo;

        public Vector3 worldPosition; 


        public ComboUpdateEvent( int currentCombo, Vector3 worldPosition)
        {
            this.currentCombo = currentCombo;
            this.worldPosition = worldPosition;
        }
    }

    public struct LevelUpEvent
    {
        public LevelDataSO currentLevelData;

        public LevelUpEvent(LevelDataSO currentLevelData)
        {
            this.currentLevelData = currentLevelData;
        }
    }

    public struct UpdateLevelBarEvent
    {
        public float fillAmount;
        public UpdateLevelBarEvent(float fillAmount)
        {
            this.fillAmount = fillAmount;
        }
    
    }


    public struct UpdateMoneyTextEvent
    {
        public float currentMoney;

        public UpdateMoneyTextEvent(float currentMoney)
        {
            this.currentMoney = currentMoney;
        }
    }

    public struct UpdateDailyIncomeTextEvent
    {
        public float dailyIncome;

        public UpdateDailyIncomeTextEvent(float dailyIncome)
        {
            this.dailyIncome = dailyIncome;
        }
    }


    public struct LevelIntroEvent
    {
        public LevelDataSO levelData;

        public LevelManager levelManager;
        public float introDuration;

        public LevelIntroEvent(LevelDataSO levelData, float introDuration , LevelManager levelManager)
        {
            this.levelData = levelData;
            this.introDuration = introDuration;
            this.levelManager = levelManager;
        }
    }

    public struct UpdateClockEvent
    {
        public int hour;
        public int minute;

        public UpdateClockEvent(int hour, int minute)
        {
            this.hour = hour;
            this.minute = minute;
        }
    }

    public struct LevelEndEvent
    {
        public LevelManager levelManager;

        public bool isLevelSuccessful;

        public LevelEndEvent (LevelManager levelManager, bool isLevelSuccessful)
        {
            this.levelManager = levelManager;
            this.isLevelSuccessful = isLevelSuccessful;
        }

    }

    public struct ShopOpeningEvent
    {
        public LevelDataSO levelData;
        public LevelManager levelManager;

        public ShopOpeningEvent( LevelDataSO levelData , LevelManager levelManager)
        {
            this.levelData = levelData;
            this.levelManager = levelManager;
        }

    }

    public struct DayStartingEvent
    {
        public LevelDataSO levelData;

        public LevelManager levelManager;
        

        public DayStartingEvent( LevelDataSO levelData , LevelManager levelManager)
        {
            this.levelData = levelData;
            this.levelManager = levelManager;
        }
   
    }

    public struct DayRushStartingEvent
    {
        public LevelManager levelManager;

        public DayRushStartingEvent( LevelManager levelManager)
        {
            this.levelManager = levelManager;
        }

    }

}