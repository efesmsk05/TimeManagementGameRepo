using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{   
    public List<Food> collectedFoods = new List<Food>();

    public GameObject[] SlotsParent;
    
    public bool isFull;




    public void PickUpFood(Food food)
    {
        if(collectedFoods.Count < 2 )
        {
            collectedFoods.Add(food);
            AddSlotVisual(food);
        }
    }

    public void ThrowTrash()
    {
        for (int i = collectedFoods.Count - 1; i >= 0; i--)
        {
            if (collectedFoods[i].isTrash)
            {
                Food trashFood = collectedFoods[i];
                collectedFoods.RemoveAt(i);
                trashFood.DeSpawn();

                DishesStation.Instance.AddDirtyDishes(1);// Çöp atıldığında otomatik bulaşık sayısı artar
                
            }
        }
    }



    public void RemoveFood(Food food)
    {
        if(collectedFoods.Contains(food) && food != null)
        {
            collectedFoods.Remove(food);

            food.transform.SetParent(null); // Slottan ayır


            food.DeSpawn();

        }
        else
        {
            Debug.Log("HATA: Silinmek istenen yemek listede yok!");
        }
    }

    private void AddSlotVisual(Food food)
    {
        for (int i = 0; i < SlotsParent.Length; i++)
        {
            if (SlotsParent[i].transform.childCount == 0)
            {
                food.transform.SetParent(SlotsParent[i].transform);
                food.transform.localPosition = Vector3.zero;
            }
        }
    }

    public int GetFoodCount()
    {
        return collectedFoods.Count;
    }

    public bool CheckFoods(CustomerController target)
    {
        foreach (var food in target.currentOrderItems)
        {
            if (food.foodSprite == target.currentOrderItems[0].foodSprite) // sahip olduğu yemek müşteri ile eşleşiyorsa
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckSlostFull()
    {
        return collectedFoods.Count >= 2;
    }

   public List<Food> FindFoodByCustomer(CustomerController target)
{
    List<Food> foodsToServe = new List<Food>();

    // Müşterinin beklediği yemeklerin geçici bir kopyasını alıyoruz.
    // Neden? Çünkü elimizdeki bir yemeği eşleştirince bu listeden düşmeliyiz ki
    // aynı sipariş için yanlışlıkla 2 yemek vermeyelim.
    List<OrderItemSO> neededItems = new List<OrderItemSO>(target.currentOrderItems);

    foreach (var foodInInventory in collectedFoods)
    {
        OrderItemSO matchedOrder = null;

        foreach (var needed in neededItems)
        {
            if (needed.foodSprite == foodInInventory.foodSprite)
            {
                matchedOrder = needed;
                break; // Eşleşen yemek bulundu
            }
        }

        if (matchedOrder != null)
        {
            foodsToServe.Add(foodInInventory); // Servis edilecekler listesine ekle
            neededItems.Remove(matchedOrder);  // İhtiyaç listesinden at
        }
    }
    return foodsToServe;
}

}
