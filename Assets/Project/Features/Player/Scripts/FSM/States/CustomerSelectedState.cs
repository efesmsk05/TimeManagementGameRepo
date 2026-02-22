using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSelectedState : PlayerState
{
    private CustomerController selectedCustomer;

    // yürüme 
    private Vector3? targetPosition; // vector3? => null olabilir
    private Action onArrivalCallback; // Varınca çalışacak kod bloğu
    public CustomerSelectedState(PlayerController playerController, CustomerController selectedCustomer , Action onArrival = null , Vector3? targetPosition = null) : base(playerController)
    {
        this.selectedCustomer = selectedCustomer;
        this.onArrivalCallback = onArrival;
        this.targetPosition = targetPosition;
    }

    public override void Enter()
    {
        EventBus.Publish(new CustomerEvents.CustomerSelectedEvent(selectedCustomer));   
    }

    public override void Update()
    {
        if (targetPosition.HasValue)
        {
            bool hasArrived = selectedCustomer.customerMovement.MoveTo(targetPosition.Value, selectedCustomer.customer.customerSpeed);

            //selectedCustomer.customerMovement.StartMoving();

            if (hasArrived)
            {
                onArrivalCallback?.Invoke();
                targetPosition = null; // Reset target position after arrival
                onArrivalCallback = null;
            }
        }

        if(selectedCustomer.currentState is LeavingShopState) // sırada beklerken başka bir masaya tıklanırsa sıradan çıkması için
        {
            playerController.ChangeState(new NormalInteractionState(playerController));
             EventBus.Publish(new CustomerEvents.CustomerDeselectedEvent(selectedCustomer));
             return;
        }
    }

    public override void Exit()
    {
    }

    public override void OnClick(Vector2 pos)
    {
        TableController clickedTable = InteractionManager.Instance.GetTableAtPosition(pos);

        if (clickedTable != null)
        {

            int groupSize = selectedCustomer.customers.Count; // gurubun kaç kişilik olduğunu alıyoruz

            if (clickedTable.CanGroupFit(groupSize)) // yer varsa   
            {

                selectedCustomer.currentTable = clickedTable; // müşteri kontrolöründeki current table'ı güncelliyoruz

                List<Transform> availableSeats = clickedTable.ReserveSeatsForGroup(selectedCustomer); // boş koltukları alıyoruz

                ////

                CustomerSpawner.Instance.OnCustomerSeated(selectedCustomer);

                selectedCustomer.ChangeState(new WalkingToTableState(selectedCustomer, availableSeats, clickedTable));
                
                EventBus.Publish(new CustomerEvents.CustomerDeselectedEvent(selectedCustomer));

                playerController.ChangeState(new NormalInteractionState(playerController));
                return;
            }
            else
            {
                Debug.Log("Bu masa grubunuz için uygun değil!");
                EventBus.Publish(new GameEvents.OnErrorEvent());
            }
        }

        if (this.onArrivalCallback != null)
        {
            Debug.Log("Müşteri seçildiği için mevcut görev ertelendi ve başa eklendi.");
            playerController.AddUrgentTask(this.targetPosition.Value, this.onArrivalCallback);
        }

        EventBus.Publish(new CustomerEvents.CustomerDeselectedEvent(selectedCustomer));
   
        playerController.ChangeState(new NormalInteractionState(playerController));

            
    }
}
