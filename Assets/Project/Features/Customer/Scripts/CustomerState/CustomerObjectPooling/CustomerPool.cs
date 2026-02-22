using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    public static CustomerPool Instance;

    [Header("Ayarlar")]
    public string customerLabel = "Customer"; 
    public int initialCountPerGroup = 3;     

    // --- HAVUZ DEPOSU ---
    // Örn: poolDictionary[2] -> 2 kişilik hazır grupların listesi
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    // --- PREFAB KAYNAĞI ---
    // Örn: prefabDictionary[4] -> 4 kişilik grup prefablarının listesi (Rastgele seçmek için)
    private Dictionary<int, List<GameObject>> prefabDictionary = new Dictionary<int, List<GameObject>>();

    public bool IsReady { get; private set; } = false;

    void Awake()
    {
        Instance = this;
        
    }

    public void InitializePool()
    {
        AddressableManager.Instance.LoadAssetByLabel<GameObject>(customerLabel, OnPrefabsLoaded);
    }

    private void OnPrefabsLoaded(IList<GameObject> loadedAssets)
    {
        foreach (GameObject prefab in loadedAssets)
        {
            int childCount = prefab.GetComponentsInChildren<Customer>(true).Length;

            // Eğer hiç bulamazsa (0 ise) varsayılan olarak 1 kabul edelim
            int size = childCount > 0 ? childCount : 1;

            if (!prefabDictionary.ContainsKey(size))
            {
                prefabDictionary[size] = new List<GameObject>();
                poolDictionary[size] = new Queue<GameObject>();
            }

            prefabDictionary[size].Add(prefab);
        }

        // Havuzları doldur
        foreach (var key in prefabDictionary.Keys)
        {
            for (int i = 0; i < initialCountPerGroup; i++)
            {
                CreatePoolItem(key);
            }
        }

        IsReady = true;
        Debug.Log("Müşteri Havuzu Hazır!");
    }

    private GameObject CreatePoolItem(int size)
    {
        // O boyutta hiç prefab yoksa (örn: hiç 3 kişilik prefab yapmadıysan) çık
        if (!prefabDictionary.ContainsKey(size) || prefabDictionary[size].Count == 0) return null;

        // O boyuttaki prefablardan rastgele birini seç
        List<GameObject> prefabs = prefabDictionary[size];
        GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];

        // Oyuna ekle ve gizle
        GameObject obj = Instantiate(randomPrefab, transform);
        obj.SetActive(false);
        
        // Havuza at
        poolDictionary[size].Enqueue(obj);
        return obj;
    }

    public GameObject GetCustomerGroup(int size, Vector3 pos, Quaternion rot) // gurup boyutu ve pozisyon bilgisi alıyoruz
    {
        if (!IsReady) return null;

        if (!poolDictionary.ContainsKey(size))
        {
            Debug.LogError($"HATA: {size} kişilik grup için prefab bulunamadı!");
            return null;
        }

        if (poolDictionary[size].Count == 0)
        {
            CreatePoolItem(size);
        }

        GameObject group = poolDictionary[size].Dequeue();
        group.transform.position = pos;
        group.transform.rotation = rot;
        group.SetActive(true);
        return group;
    }

    // Müşteri dükkandan çıkınca buraya gönderilecek
    public void ReturnCustomer(GameObject customerGroup)
    {
        customerGroup.SetActive(false);
        
        // Bu grup kaç kişilikti? (Controller'dan bakıyoruz)
        CustomerController controller = customerGroup.GetComponent<CustomerController>();
        int size = (controller.customers != null && controller.customers.Count > 0) ? controller.customers.Count : 1;

        if (poolDictionary.ContainsKey(size))
        {
            poolDictionary[size].Enqueue(customerGroup);
        }
        else
        {
            Destroy(customerGroup); // Tanımsızsa yok et
        }
    }
}