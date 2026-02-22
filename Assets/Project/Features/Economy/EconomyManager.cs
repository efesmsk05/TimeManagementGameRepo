using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{

    public static EconomyManager Instance;
    public float dailyIncome;

    public List<float> dailyIncomes = new List<float>();



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        EventBus.Subscribe<PlayerEvents.OnMoneyCollectedEvent>(HandleMoneyCollected);

    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlayerEvents.OnMoneyCollectedEvent>(HandleMoneyCollected);
    }
    void Start()
    {
        dailyIncome = 0;
    }


    private void HandleMoneyCollected(PlayerEvents.OnMoneyCollectedEvent evt)
    {
        dailyIncome += evt.amount;
        EventBus.Publish(new UIEvent.UpdateDailyIncomeTextEvent(dailyIncome));

        GameDataManager.Instance.AddMoney(evt.amount);
        Debug.Log("Para toplandı: " + evt.amount + " Günlük gelir: " + dailyIncome);
    }

    // function to add income
 
    public void ReduceIncome(float amount)// gelir azalt
    {
        dailyIncome -= amount;
        if (dailyIncome < 0)
        {
            dailyIncome = 0;
        }
        EventBus.Publish(new UIEvent.UpdateDailyIncomeTextEvent(dailyIncome));
    }

    public void EndOfDay(LevelManager levelManager ) // gün sonu
    {
        dailyIncomes.Add(dailyIncome);
        
        if(dailyIncome >= levelManager.targetDailyIncome)
        {
            EventBus.Publish(new UIEvent.LevelEndEvent(levelManager, true));
        }
        else
        {
            EventBus.Publish(new UIEvent.LevelEndEvent(levelManager, false));
        }

        dailyIncome = 0;
        
    }





}
