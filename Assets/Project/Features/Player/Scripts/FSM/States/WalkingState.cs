using System;
using UnityEngine;

public class WalkingState : PlayerState
{
    private Vector3 targetPosition;
    private Action onArrivalCallback;

    public WalkingState(PlayerController controller, Vector3 target, Action onArrival = null) 
        : base(controller)
    {
        this.targetPosition = target;
        this.onArrivalCallback = onArrival;
    }

    public override void Enter()
    {
        playerController.animator.SetBool("IsWalking", true);
        DishesStation.Instance.CancelWashing(); 
    }

    public override void Update()
    {
        bool hasArrived = playerController.playerMovement.MovePlayer(targetPosition);

        if (hasArrived)
        {
            onArrivalCallback?.Invoke();

            playerController.ChangeState(new NormalInteractionState(playerController));
        }
    }

    public override void OnClick(Vector2 pos) // Bu durumlar karakter bir komut esnasında iken verilen komutları yönetir
    {
        // --- 1. ACİL DURUM: MÜŞTERİ SEÇİMİ ---
        CustomerController clickedCustomer = InteractionManager.Instance.GetCustomerAtPosition(pos);
        if(clickedCustomer != null && clickedCustomer.currentState is WaitingForSeatState)
        {
            // KRİTİK BÖLÜM: YARIM KALAN İŞİ KURTARMA
            // Eğer şu an bir yere gidiyorsam ve yapacak bir işim varsa,
            // bu işi unutmamak için listenin EN BAŞINA geri ekle (AddUrgentTask).
            if (this.onArrivalCallback != null)
            {
                //Debug.Log("Müşteri seçildiği için mevcut görev ertelendi ve başa eklendi.");
                playerController.AddUrgentTask(this.targetPosition, this.onArrivalCallback);
            }
            
            playerController.ChangeState(new CustomerSelectedState(playerController, clickedCustomer));
            return;
        }

        // --- 2. YENİ GÖREV EKLEME (KUYRUK SİSTEMİ) ---
        
        // Masa Tıklaması
        TableController clickedTable = InteractionManager.Instance.GetTableAtPosition(pos);
        if (clickedTable != null)
        {
            clickedTable.SelectTableEffect(); // Masa seçme efekti

            Vector3 target = clickedTable.interactionPoint.position;

            // Listeye ekle: Git ve SmartLogic çalıştır
            playerController.AddTask(target, () => 
            {
                playerController.SmartTableInteraction(clickedTable);
            });
            return;
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

        // Yemek Tıklaması
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
                    }
                });
            }
            return;
        }
    }

    public override void Exit() 
    {
        playerController.animator.SetBool("IsWalking", false);
    }
}