using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WaitingForOrderState : TimedCustomerState
{
    public float orderWaitTime = 7f; 
    private int groupFoodCount;
    private int currentCustomerIndexToServe = 0; 

    public WaitingForOrderState(CustomerController controller ) : base(controller)
    {
    }

    public override float GetTotalTime()
    { 
        float tempWaitTime = 0f; 

        foreach(var item in customerController.currentOrderItems)
        {
            tempWaitTime += item.prepTime; 
        }

        tempWaitTime += (customerController.customer.patienceMultiplier * orderWaitTime);
        return tempWaitTime;
    }

    protected override PatienceBarType GetPatienceBarType()
    {
        return PatienceBarType.orderWait; 
    }
 
    public override void Enter()
    {
        groupFoodCount = customerController.customers.Count; 
        currentCustomerIndexToServe = 0; 

        float exactWaitTime = GetTotalTime(); 

        if(customerController.currentTable != null)
        {
            customerController.currentTable.ShowWaitingForFoodVisuals(exactWaitTime, customerController.currentOrderItems); 
        }

        base.Enter();
    }

    public void TakedFood(List<Food> foods) 
    {
        if(customerController.currentTable != null) 
        {
            for (int i = 0; i < foods.Count; i++)
            {
                // Güvenlik
                if (currentCustomerIndexToServe >= customerController.customers.Count) break;

                // Doğru koltuk indeksini alıyoruz
                int targetSeatIndex = customerController.customers[currentCustomerIndexToServe].seatIndex;

                customerController.currentTable.ShowFood( 
                    targetSeatIndex,  
                    foods[i].foodSprite
                );

                customerController.currentTable.visuals.MarkFoodAsServed(foods[i].foodSprite);

                currentCustomerIndexToServe++; 
                groupFoodCount--;
            }
        }

        if (groupFoodCount <= 0)
        {
            Debug.Log("Tüm yemekler alındı, müşteriler yemeye başlayacak!");
            EventBus.Publish(new CustomerEvents.FoodServedEvent(customerController));
            customerController.ChangeState(new EatingFoodState(customerController));
        }
    }

    protected override void OnTimeout()
    {
        Debug.Log("Sipariş beklemekten sıkıldı, gidiyor!");

        customerController.HidePatienceBar();

        if (customerController.currentTable != null)
        {
            customerController.currentTable.CleanTable();
            
        }

        EventBus.Publish(new CustomerEvents.CustomerLeftEvent(customerController));

        // burada önce ui in kapanmasını beklemeli sonra masadan kalkmalı çünkü ui kapanmadan masadan kalkarsa müşteri hala masada görünebilir
         UniTask.Delay(500).ContinueWith(() => 
         {
             customerController.ChangeState(new LeavingShopState(customerController));
         });
    }

    public override void Update() {}
    public override void Exit() { base.Exit(); }
}