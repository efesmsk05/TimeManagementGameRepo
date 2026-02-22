using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodPool : MonoBehaviour
{
    public static FoodPool Instance { get; private set; }
    
    public GameObject foodPrefab;

    public int poolSize = 10;

    public Queue<GameObject> foodQueue = new Queue<GameObject>();


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
        InitializePool();
    }
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject foodItem = Instantiate(foodPrefab, transform);
            foodItem.SetActive(false);
            foodQueue.Enqueue(foodItem);
        }
    }

    public GameObject GetFoodItem()
    {
        if (foodQueue.Count == 0)
        {
            Debug.LogWarning("Food pool is empty!");
            return null;
        }

        GameObject foodItem = foodQueue.Dequeue();
        foodItem.SetActive(true);
        return foodItem;
    }

    public void ReturnFoodItem(GameObject foodItem)
    {
        foodItem.SetActive(false);
        foodItem.transform.SetParent(transform);
        foodQueue.Enqueue(foodItem);
    }




}
