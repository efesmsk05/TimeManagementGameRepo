using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;

    [Header("Settings")]
    public GameObject orderReadyPrefab;
    public Transform orderReadySpawnPoint;
    public float kitchenSpeed = 1f;

    [Header("Kitchen Capacity")]
    private int maxStoveSlots = 2; 
    private int busyStoveSlots = 0; 

    private Queue<KitchenTask> foodQueue = new Queue<KitchenTask>();

    private int foodCount = 0; // Spawn pozisyonu için sayaç
    private CancellationTokenSource cts = new CancellationTokenSource();

    // Tek bir yemek pişirme görevi için küçük bir sınıf
    public class KitchenTask
    {
        public OrderItemSO foodData;
        public CustomerController owner;
        public float prepTime;
    }

    void OnEnable() => EventBus.Subscribe<DataEvents.OnUpgradeSuccessEvent>(UpdateKitchenSpeed);
    void OnDisable() => EventBus.Unsubscribe<DataEvents.OnUpgradeSuccessEvent>(UpdateKitchenSpeed);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddOrder(CustomerController customer, List<OrderItemSO> orderData)
    {
        if (customer == null || orderData == null) return;

        // Bulaşık Kontrolü
        if (DishesStation.Instance.TryGetCleanDish(orderData.Count) == false)
        {
            Debug.Log("Tabak yok, sipariş alınamadı.");
            return;
        }

        List<float> prepTimes = GetFoodPrepTime(orderData);

        for (int i = 0; i < orderData.Count; i++)
        {
            KitchenTask newTask = new KitchenTask();
            newTask.foodData = orderData[i];
            newTask.owner = customer;
            newTask.prepTime = prepTimes[i];

            foodQueue.Enqueue(newTask);
        }

        TryStartCooking();
    }

    private void TryStartCooking()
    {
        while (foodQueue.Count > 0 && busyStoveSlots < maxStoveSlots)
        {
            // Sıradaki yemeği al
            KitchenTask task = foodQueue.Dequeue();
            
            // Ocağın bir gözünü doldur
            busyStoveSlots++; 

            // Pişirme işlemini başlat (Asenkron)
            CookSingleFoodAsync(task, cts.Token).Forget();
        }
    }

    private async UniTaskVoid CookSingleFoodAsync(KitchenTask task, CancellationToken token)
    {
        Debug.Log($"Pişiriliyor: {task.foodData.foodName}, Süre: {task.prepTime} saniye");


        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(task.prepTime), cancellationToken: token).SuppressCancellationThrow();

        if (isCancelled) return;

        if (task.owner == null || !(task.owner.currentState is WaitingForOrderState))
        {
            Debug.Log("Müşteri gitmiş! Yemek çöpe, tabak geri.");
            
            DishesStation.Instance.cleanDishesCount++;
            
            busyStoveSlots--;
            TryStartCooking();
            return;
        }

        OrderReady(task.foodData);

        busyStoveSlots--;
        
        TryStartCooking();
    }

    public void OrderReady(OrderItemSO completedOrder)
    {
        if (foodCount >= 5) foodCount = 0;
        foodCount++;

        Vector3 pos = new Vector3(orderReadySpawnPoint.position.x + foodCount * 1.2f, orderReadySpawnPoint.position.y, orderReadySpawnPoint.position.z);

        GameObject readyOrder = FoodPool.Instance.GetFoodItem();
        readyOrder.transform.position = pos;

        Food foodComponent = readyOrder.GetComponent<Food>();
        if (foodComponent != null)
        {
            foodComponent.Initialize(completedOrder);

        }



    }

    public void UpgradeStoveSlot()
    {
        maxStoveSlots++; // Kapasiteyi artır
        Debug.Log($"Ocak Geliştirildi! Artık aynı anda {maxStoveSlots} yemek pişiyor.");
        
        TryStartCooking(); // Bekleyen varsa hemen yeni göze al
    }

    // --- YARDIMCI FONKSİYONLAR ---
    private void UpdateKitchenSpeed(DataEvents.OnUpgradeSuccessEvent evt)
    {
        if (evt.upgradeType == UpgradeType.KitchenSpeed)
        {
            kitchenSpeed = GameDataManager.Instance.GetUpgradeValues(UpgradeType.KitchenSpeed);
        }
    }

    private List<float> GetFoodPrepTime(List<OrderItemSO> orderItems)
    {
        List<float> prepTimes = new List<float>();
        foreach (var item in orderItems)
        {
            prepTimes.Add(item.prepTime / kitchenSpeed);
        }
        return prepTimes;
    }

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}