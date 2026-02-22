using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance;

    [Header("Spawn Points")]
    public Transform spawnPoint;
    public Transform customerTargetPoint;

    [Header("--- NORMAL MODE SETTINGS ---")]
    public int normalGroupSizeMin = 1; // Normal zamanlarda tekil müşteriler veya küçük gruplar (2 kişiye kadar) gelebilir
    public int normalGroupSizeMax = 2;
    
    public float normalGroupIntervalMin = 5f; // spawn süre arası 
    public float normalGroupIntervalMax = 10f;

    [Header("--- RUSH HOUR SETTINGS ---")]
    public int rushGroupSizeMin = 2; // Rush saatlerinde daha büyük gruplar (2-4 kişi) gelebilir
    public int rushGroupSizeMax = 4;

    public float rushGroupIntervalMin = 2f;
    public float rushGroupIntervalMax = 4f;

    [Header("--- Status ---")]
    private bool isRushHourActive = false;
    public bool isSpawningActive;

    [Header("Level Settings")]
    private int totalCustomersToSpawn; 
    private int spawnedCount = 0;      

    [Header("Queue Settings")]
    public float queueDistance = 1.0f;
    public Vector3 queueDirection = new Vector3(0, -1, 0);
    public List<CustomerController> waitingCustomers = new List<CustomerController>();

    private CancellationTokenSource cts;

    void OnEnable() => EventBus.Subscribe<GameEvents.RushTimeEvent>(OnRushTimeEvent);
    void OnDisable() => EventBus.Unsubscribe<GameEvents.RushTimeEvent>(OnRushTimeEvent);

    void Awake()
    {
        Instance = this;
    }

    public void InitializeLevel(int totalCount)
    {

        // Rush interval'ı normale göre daha agresif yapıyoruz
        this.rushGroupIntervalMin = Mathf.Max(1f, normalGroupIntervalMin / 3f);
        this.rushGroupIntervalMax = Mathf.Max(2f, normalGroupIntervalMax / 3f);

        CalculateGroupSizesForLevel();

        totalCustomersToSpawn = totalCount;
        spawnedCount = 0;
        isRushHourActive = false;
    }

    public void StartSpawning() 
    { 
        isSpawningActive = true; 
        if(cts != null) cts.Cancel();
        cts = new CancellationTokenSource();
        SpawnRoutine(cts.Token).Forget();
    }

    public void StopSpawning() 
    { 
        isSpawningActive = false; 
        ResetTokenSource();
    }

    private void ResetTokenSource()
    {
        if(cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }


    private void CalculateGroupSizesForLevel() // Level tasarımına göre grup boyutlarını ayarlar
    {
        

        TableController[] tables = FindObjectsOfType<TableController>(); // sahnedeki tüm masaları bul  

        int maxCapacity = 0;

        foreach (var table in tables)
        {
            if(table != null && table.seatPoints != null && table.seatPoints.Length > maxCapacity)// maks masa kapasitesi
            {
                maxCapacity = table.seatPoints.Length;
            }   

        }

        if(normalGroupSizeMax > maxCapacity)// max grup sayısı kontrol ve ayarlaması NORMAL
        {
            normalGroupSizeMax = maxCapacity;
        }

        if(rushGroupSizeMax > maxCapacity) // RUSH
        {
            rushGroupSizeMax = maxCapacity;
        }

        Debug.Log($"Masa kapasitesi: {maxCapacity}. Normal Grup Boyutu: {normalGroupSizeMin}-{normalGroupSizeMax}, Rush Grup Boyutu: {rushGroupSizeMin}-{rushGroupSizeMax}");


    }



    private async UniTaskVoid SpawnRoutine(CancellationToken token)
    {
        Debug.Log($" Spawn Hedef: {totalCustomersToSpawn}");
        
        await UniTask.Delay(System.TimeSpan.FromSeconds(2f), cancellationToken: token).SuppressCancellationThrow();

        while (isSpawningActive && spawnedCount < totalCustomersToSpawn)
        {

            int currentGroupSize;
            
            if (isRushHourActive)
                currentGroupSize = Random.Range(rushGroupSizeMin, rushGroupSizeMax + 1);
            else
                currentGroupSize = Random.Range(normalGroupSizeMin, normalGroupSizeMax + 1);

            int remainingCustomers = totalCustomersToSpawn - spawnedCount; // kalan müşteri
            if (currentGroupSize > remainingCustomers) 
                currentGroupSize = remainingCustomers;

                SpawnCustomerGroup(currentGroupSize);
                spawnedCount++;

                await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0.3f, 0.6f)), cancellationToken: token).SuppressCancellationThrow();

            Debug.Log($"📦 Grup Girdi: {currentGroupSize} kişi. Toplam: {spawnedCount}/{totalCustomersToSpawn}");

            if (spawnedCount >= totalCustomersToSpawn) break;

            //Spawn wait
            float waitTime;
            if (isRushHourActive)
                waitTime = Random.Range(rushGroupIntervalMin, rushGroupIntervalMax); // Rush
            else
                waitTime = Random.Range(normalGroupIntervalMin, normalGroupIntervalMax); // Normal

            await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token).SuppressCancellationThrow();
        }
        
        Debug.Log("Total customer spawnı bitti");
        isSpawningActive = false;
    }

    private void SpawnCustomerGroup(int groupSize)
    {
        GameObject customerObj = CustomerPool.Instance.GetCustomerGroup(groupSize, spawnPoint.position, Quaternion.identity); 
        
        if (customerObj != null)
        {
            CustomerController controller = customerObj.GetComponent<CustomerController>();
            if (controller != null)
            {
                waitingCustomers.Add(controller);
                Vector3 targetPos = GetQueuePosition(waitingCustomers.Count - 1);
                controller.Initialize(targetPos);
            }
        }
        else
        {
            Debug.LogError($"HATA: {groupSize} kişilik grup spawn edilemedi! Pool'da bu boyutta prefab var mı?");
        }
    }
    private void OnRushTimeEvent(GameEvents.RushTimeEvent rushEvent)
    {
        isRushHourActive = rushEvent.isRushHour;
        if(isRushHourActive) Debug.Log("🔥 RUSH BAŞLADI: Gruplar büyüdü, aralıklar kısaldı!");
        else Debug.Log("✅ RUSH BİTTİ: Sakinliğe dönüş.");
    }

    private Vector3 GetQueuePosition(int index) => customerTargetPoint.position + (queueDirection.normalized * (queueDistance * index));

    public void OnCustomerSeated(CustomerController customer)
    {
        if(waitingCustomers.Contains(customer))
        {   
            waitingCustomers.Remove(customer);
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < waitingCustomers.Count; i++)
        {
            waitingCustomers[i].UpdateTargetPosition(GetQueuePosition(i));
        }
    }

    public void OnCustomerLeft(CustomerController customer)
    {
        if(waitingCustomers.Contains(customer))
        {   
            waitingCustomers.Remove(customer);
            UpdateQueuePositions();
        }
    }
}