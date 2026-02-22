using UnityEngine;

public class NormalInteractionState : PlayerState
{
    public NormalInteractionState(PlayerController playerController) : base(playerController)
    {
    }

    public override void Enter()
    {
        if (playerController.taskList.Count == 0)
        {
            playerController.animator.SetBool("IsIdleing", true);
        }
    }

    public override void Update() 
    { 
        if (playerController.taskList.Count > 0) // Eğer sıraya alınmış bir görev varsa
        {
            // 1. Listenin en başındaki görevi al
            PlayerTask nextTask = playerController.taskList[0];

            // 2. Listeden sil 
            playerController.taskList.RemoveAt(0);

            // 3. Görevi yapmak için WalkingState'e geç
            playerController.ChangeState(new WalkingState(
                playerController, 
                nextTask.targetPosition, 
                nextTask.onArrivalAction
            ));
        }
    }

    public override void Exit() 
    {  
        playerController.animator.SetBool("IsIdleing", false);
    }

    public override void OnClick(Vector2 pos)
    {
        // 1. MÜŞTERİ SEÇİMİ (ACİL)
        CustomerController clickedCustomer = InteractionManager.Instance.GetCustomerAtPosition(pos);
        if(clickedCustomer != null && clickedCustomer.currentState is WaitingForSeatState)
        {

            var waitingCustomers = CustomerSpawner.Instance.waitingCustomers;

            if(waitingCustomers.Count > 0 && waitingCustomers[0] == clickedCustomer)
            {
                playerController.ClearTasks();
                playerController.ChangeState(new CustomerSelectedState(playerController, clickedCustomer));
            
            }
            else
            {
                Debug.Log("Bu müşteri sırada ilk değil!");
            }

            return;

            // Müşteri seçimi acil olduğu için mevcut listeyi temizle
      
        }   


        Trash clickedTrash = InteractionManager.Instance.GetTrashAtPosition(pos);
        if (clickedTrash != null)
        {

            playerController.AddTask(clickedTrash.transform.position, () => 
            {

                clickedTrash.Interact(playerController.playerInventory);
            });
            return;
        }

        DishesStation clickedDishesStation = InteractionManager.Instance.GetDishesStation(pos);
        if (clickedDishesStation != null)
        {
            playerController.AddTask(clickedDishesStation.washPointOffset.position, () =>
            {
                
                clickedDishesStation.WashDishes();


            });
            return;
        }



        // 2. YEMEK ALMA
        Food clickedFood = InteractionManager.Instance.GetFoodAtPosition(pos);
        if (clickedFood != null)
        {
            if (!playerController.playerInventory.CheckSlostFull())
            {
                playerController.AddTask(clickedFood.transform.position, () => 
                {
                    if(!playerController.playerInventory.CheckSlostFull())
                    {
                        playerController.playerInventory.PickUpFood(clickedFood);
                        clickedFood.StopFoodEffect();
                    }
                });
            }
            return;
        }

        TableController clickedTable = InteractionManager.Instance.GetTableAtPosition(pos);
        if (clickedTable != null)
        {
            clickedTable.SelectTableEffect(); // Masa seçme efekti

            Vector3 target = clickedTable.interactionPoint.position;

            playerController.AddTask(target, () => 
            {
                playerController.SmartTableInteraction(clickedTable); // bu genel mantık artık burada player controller artık genel yönetici
            });
            return;
        }
    }
}