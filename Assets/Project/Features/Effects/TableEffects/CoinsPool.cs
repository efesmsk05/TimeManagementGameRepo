using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsPool : MonoBehaviour
{
    public static CoinsPool Instance { get; private set; }

    private string coinLabel = "Coin";
    [SerializeField] private int initialPoolSize = 10;

    private GameObject coinPrefab;
    private Queue<GameObject> coinsQueue = new Queue<GameObject>();

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

    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        AddressableManager.Instance.LoadAssetByLabel<GameObject>(coinLabel, OnPrefabsLoaded);
    }

    private void OnPrefabsLoaded(IList<GameObject> loadedAssets)
    {
        if (loadedAssets.Count > 0)
        {
            coinPrefab = loadedAssets[0]; 
            
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewCoin();
            }
        }
        else
        {
            Debug.LogError("No coin prefabs found with label: " + coinLabel);
        }
    }

    // Kod tekrarını önlemek için coin üretme işlemini ayırdık
    private void CreateNewCoin()
    {
        GameObject coinItem = Instantiate(coinPrefab, transform);
        coinItem.SetActive(false);
        coinsQueue.Enqueue(coinItem);
    }

    public GameObject GetCoinItem()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("Coin prefab is still loading...");
            return null; 
        }

        if (coinsQueue.Count == 0)
        {
            CreateNewCoin();
        }

        GameObject coinItem = coinsQueue.Dequeue();
        coinItem.SetActive(true);
        return coinItem;
    }

    public void ReturnCoinItem(GameObject coinItem)
    {
        if (!coinItem.activeSelf) return;

        coinItem.SetActive(false);
        coinItem.transform.SetParent(transform);
        coinsQueue.Enqueue(coinItem);
    }
}