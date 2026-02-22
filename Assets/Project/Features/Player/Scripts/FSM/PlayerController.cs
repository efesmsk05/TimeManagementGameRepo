using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public PlayerMovement playerMovement;
    public PlayerInventory playerInventory;
    public PlayerState currentState;
    [SerializeField] public Animator animator; 

    [Header("Player Values")]
    public float playerSpeed = 7f;

    public List<PlayerTask> taskList = new List<PlayerTask>();

    void OnEnable()
    {
        EventBus.Subscribe<DataEvents.OnUpgradeSuccessEvent>(OnUpgradeSuccessEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DataEvents.OnUpgradeSuccessEvent>(OnUpgradeSuccessEvent);
    }




    public void AddTask(Vector3 pos, System.Action action)
    {
        // Standart ekleme: Listenin SONUNA ekler.
        taskList.Add(new PlayerTask(pos, action));
    }

    public void AddUrgentTask(Vector3 pos, System.Action action)
    {
        taskList.Insert(0, new PlayerTask(pos, action));
    }

    public void ClearTasks()
    {
        taskList.Clear();
    }
    
    //  masa mantığı
    public void SmartTableInteraction(TableController table)
    {

        // Yemek servisi
        if (table.CheckCustomerFoods()) 
        {
            var targetTableGroup = table.GetCustomerMatchingFood(playerInventory);
            
            if(targetTableGroup != null)
            {

                List<Food> foodsToServe = playerInventory.FindFoodByCustomer(targetTableGroup);


                if (foodsToServe != null && foodsToServe.Count > 0 && targetTableGroup.currentState is WaitingForOrderState orderState)
                {
                    // buraya elimizde ne kadar yemek var kontrolü eklenebilir, eğer yemek yoksa hata mesajı verilebilir
                    orderState.TakedFood(foodsToServe);

                    foodsToServe.ForEach(food => food.ResetFoodVisual());

                    foreach (var food in foodsToServe)
                    {
                        playerInventory.RemoveFood(food);
                    }

                    EventBus.Publish(new PlayerEvents.ComboTriggeredEvent(table.transform.position));

                    return; 
                }
                else
                {
                    Debug.Log("Masadaki müşteriler için uygun yemeği envanterinizde bulamadınız!");
                    EventBus.Publish(new GameEvents.OnErrorEvent());
                }
  
        
            }

        }


        // Sipariş alma
        if (table.CheckCustomersOrder())
        {
            table.TakeCustomersOrder();
            return;
        }

        // Masa temizleme
        if (table.HasDirtyPlates())
        {
            
            EventBus.Publish(new PlayerEvents.ComboTriggeredEvent(table.transform.position));
            
            table.CleanAllDirtyPlates();
            return;
        }

    }

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInventory = GetComponent<PlayerInventory>();
        SyncPlayerStats();

        ChangeState(new NormalInteractionState(this));   
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        if (currentState != null) currentState.Enter();
    }

    private void OnUpgradeSuccessEvent(DataEvents.OnUpgradeSuccessEvent evt)
    {
        if(evt.upgradeType == UpgradeType.PlayerSpeed)
        {
            playerSpeed = GameDataManager.Instance.GetUpgradeValues(UpgradeType.PlayerSpeed);

            Debug.Log($"Player Speed upgrade alındı! Yeni Hız: {playerSpeed}");
            

        }
    }

    private void SyncPlayerStats()
    {
        // Manager'dan kayıtlı hızı iste
        float savedSpeed = GameDataManager.Instance.GetUpgradeValues(UpgradeType.PlayerSpeed);

        // Eğer veri geldiyse (0 değilse) uygula
        if (savedSpeed > 0)
        {
            playerSpeed = savedSpeed;
            Debug.Log($"Oyun Başladı: Kayıtlı Hız Yüklendi -> {playerSpeed}");
        }
        
        // Bu hızı NavMeshAgent'a da bildirmeyi unutma!
        if(playerMovement != null && playerMovement.agent != null)
        {
            playerMovement.agent.speed = playerSpeed;
        }
    }
}