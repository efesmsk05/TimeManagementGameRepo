using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EatingFoodState : TimedCustomerState
{
    // Base constructor'a 3 saniye (yeme süresi) gönderiyoruz.

    private float eatingTime = 3f; // Örnek yeme süresi
    public EatingFoodState(CustomerController customerController) : base(customerController) //sonradan buraya yeme süresi artı yemeğin ortalama yeme süresi hesabu gelicek
    {
    }

    public override float GetTotalTime()
    {
        return eatingTime; // Buraya yeme süresi artı yemeğin ortalama yeme süresi hesabu gelicek
    }


    public override void Enter()
    {
        base.Enter();

        EventBus.Publish(new CustomerEvents.CustomerStartEatingEvent(customerController));

      
    }

    // --- DÜZELTME BURADA ---
    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }

     protected override void OnTimeout()
    {

        customerController.currentTable.AddMoneyToTable(customerController.currentOrderItems);

        customerController.currentTable.MakePlateDirty();

        customerController.currentTable.ReleaseTable();


        float randomXP = Random.Range(5f, 15f); // rastgele xp miktarı
        EventBus.Publish(new DataEvents.OnReputationGainedEvent( randomXP));

        customerController.ChangeState(new LeavingShopState(customerController));
        
    }


}