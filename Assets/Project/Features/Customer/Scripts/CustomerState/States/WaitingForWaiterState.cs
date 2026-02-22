using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class WaitingForWaiterState : TimedCustomerState
{

    private float baseWaitTime = 20; // Temel bekleme süresi
    public float totalWaitTime ; 


    public WaitingForWaiterState(CustomerController customerController): base(customerController )
    {
    }
    public override float GetTotalTime()
    {
        totalWaitTime = baseWaitTime * customerController.customer.patienceMultiplier; // örneğin 1.2 ise, 4 saniye * 1.2 = 4.8 saniye bekleyecek veya 0.8 ise 4 saniye * 0.8 = 3.2 saniye bekleyecek
        return totalWaitTime;   
    }

    protected override PatienceBarType GetPatienceBarType()
    {
        return PatienceBarType.waiterWait; // hangi bar çalışıcak
    }

    public override void Enter()
    {

        if(customerController.currentTable != null)// masada oturuyorsa
        {
            customerController.currentTable.ShowReadyToOrderVisuals(GetTotalTime()); 
        }
        base.Enter();
    }
    public override void Update()
    {

    }


    protected override void OnTimeout()
    {
        Debug.Log("Sipariş vermekten sıkıldı, gidiyor!");

        customerController.currentTable.ReleaseTable(); // masadaki yeri boşaltıyoruz

        EventBus.Publish(new CustomerEvents.CustomerLeftEvent(customerController));

        customerController.ChangeState(new LeavingShopState(customerController));
        // customerController.ChangeState(new AngryLeavingState(customerController));
    }

    public void TakeOrder()
    {
        customerController.ChangeState(new WaitingForOrderState(customerController ));

    }

    public override void Exit()
    {
        base.Exit();
    }

}