    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using PrimeTween;

    public class TableController : MonoBehaviour
    {
        [Header("Table Slots")]
        [SerializeField] public Transform[] seatPoints;

        [Header("Visual Controller")]
        [SerializeField] public TableVisualController visuals; // <--- TEK DEĞİŞİKLİK: Bağlantı noktası

        [Header("Coins")]
        public List<GameObject> activeCoins = new List<GameObject>(); 

        private CustomerController[] seatedCustomersGroup;
        public bool[] slotOccupied;

        public Transform interactionPoint;
        // spriteRenderer değişkeni VisualController'a taşındı.

        private float currentMoneyOnTable = 0f;

        void Awake()
        {
            if (visuals == null) visuals = GetComponent<TableVisualController>();

            seatedCustomersGroup = new CustomerController[seatPoints.Length];
            slotOccupied = new bool[seatPoints.Length];
            
        }

        void OnEnable()
        {
            EventBus.Subscribe<CustomerEvents.CustomerSelectedEvent>(HandleCustomerSelectedVisuals);
            EventBus.Subscribe<CustomerEvents.CustomerDeselectedEvent>(HandleCustomerDeselectedVisuals);
            EventBus.Subscribe<CustomerEvents.CustomerMakeDecisionEvent>(HandleCustomerOrderDecision);
            EventBus.Subscribe<CustomerEvents.FoodServedEvent>(HandleFoodServed);
            EventBus.Subscribe<CustomerEvents.CustomerLeftEvent>(HandleCustomerLeft);
        }

        void OnDisable()
        {
            EventBus.Unsubscribe<CustomerEvents.CustomerSelectedEvent>(HandleCustomerSelectedVisuals);
            EventBus.Unsubscribe<CustomerEvents.CustomerDeselectedEvent>(HandleCustomerDeselectedVisuals);
            EventBus.Unsubscribe<CustomerEvents.CustomerMakeDecisionEvent>(HandleCustomerOrderDecision);
            EventBus.Unsubscribe<CustomerEvents.FoodServedEvent>(HandleFoodServed);
            EventBus.Unsubscribe<CustomerEvents.CustomerLeftEvent>(HandleCustomerLeft);
        }

        public bool CheckCustomersOrder()
        {
            if (seatedCustomersGroup == null || seatedCustomersGroup.Length == 0) return false;
            for (int i = 0; i < seatedCustomersGroup.Length; i++)
            {
                CustomerController customer = seatedCustomersGroup[i];
                if (customer != null && customer.currentState is WaitingForWaiterState) return true;
            }
            return false;
        }

        public void TakeCustomersOrder()
        {
            bool anyOrderTaken = false;

            for (int i = 0; i < seatedCustomersGroup.Length; i++)
            {
                CustomerController customer = seatedCustomersGroup[i];
                if (customer != null && DishesStation.Instance.isHaveCleanDishes(customer.customers.Count))
                {
                    if (customer.currentState is WaitingForWaiterState)
                    {
                        var state = customer.currentState as WaitingForWaiterState;
                        state.TakeOrder();
                        EventBus.Publish(new CustomerEvents.OrderTakenEvent(customer));

                        KitchenManager.Instance.AddOrder(customer, customer.currentOrderItems);
                        
                        EventBus.Publish(new PlayerEvents.ComboTriggeredEvent(transform.position));
                        anyOrderTaken = true;
                    }
                }
                else if (customer != null && customer.currentState is WaitingForWaiterState && !DishesStation.Instance.isHaveCleanDishes(customer.customers.Count))
                {
                    Debug.Log("Masanızdaki müşteriler için yeterli temiz tabak yok!");
                    EventBus.Publish(new GameEvents.OnErrorEvent());
                }

 
            }

            if (anyOrderTaken)// değişti düzenelecek
            {
                // Sabır barı ve görsel güncelleme
                //ShowWaitingForFoodVisuals(10f); // Süreyi yine buradan gönderiyoruz
            }
        }

        public CustomerController OwnerOfFood()
        {
            for (int i = 0; i < seatedCustomersGroup.Length; i++)
            {
                CustomerController customer = seatedCustomersGroup[i];
                if (customer != null && customer.currentState is WaitingForOrderState) return customer;
            }
            return null;
        }
        
        public CustomerController GetCustomerMatchingFood(PlayerInventory inventory)
        {
            foreach (var customer in seatedCustomersGroup)
            {
                if (customer != null && customer.currentState is WaitingForOrderState)
                {
                    if (inventory.CheckFoods(customer)) return customer;
                }
            }
            return null;
        }

        public bool CheckCustomerFoods()
        {
            if (seatedCustomersGroup == null || seatedCustomersGroup.Length == 0) return false;
            for (int i = 0; i < seatedCustomersGroup.Length; i++)
            {
                CustomerController customer = seatedCustomersGroup[i];
                if (customer != null && customer.currentState is WaitingForOrderState) return true;
            }
            return false;
        }

        public bool CanGroupFit(int groupSize)
        {
            int emptySeats = 0;
            for (int i = 0; i < slotOccupied.Length; i++)
            {
                if (!slotOccupied[i]) emptySeats++;
            }
            return emptySeats >= groupSize;
        }

        public List<Transform> ReserveSeatsForGroup(CustomerController groupController)
        {
            List<Transform> reservedSeats = new List<Transform>();
            int reservedSeatsCount = 0;
            int groupSize = groupController.customers.Count;

            for (int i = 0; i < slotOccupied.Length; i++)
            {
                if (!slotOccupied[i])
                {
                    slotOccupied[i] = true;
                    seatedCustomersGroup[i] = groupController;
                    groupController.customers[reservedSeatsCount].seatIndex = i;
                    reservedSeats.Add(seatPoints[i]);
                    reservedSeatsCount++;
                    if (reservedSeatsCount >= groupSize) break;
                }
            }
            return reservedSeats;
        }

        public void ReleaseTable()
        {
            for (int i = 0; i < slotOccupied.Length; i++)
            {
                slotOccupied[i] = false;
                seatedCustomersGroup[i] = null;
            }
        }

        public void CleanTable()
        {
            // masdaki aktif tabakları pis yapar

            ReleaseTable();

            visuals.MakeDirtyToActivePlates();
            currentMoneyOnTable = 0f;
            
            // Görsel temizliği devret
            visuals.ResetAllVisuals();
        }

        public void ShowFood(int index, Sprite foodSprite)
        {
            // Görseli VisualController yönetir
            visuals.SetFoodOnPlate(index, foodSprite);
        }

        public void MakePlateDirty()
        {
            Debug.Log("Masa üzerindeki tabaklar kirleniyor...");

            int coinAmount;

            if(currentMoneyOnTable > 0f)
            {
                AddCoinEffect(5);

            }
            else
            {
                coinAmount = 0;
                Debug.Log("Masada para yok, coin efekti atlanacak.");
                
            }

            visuals.SetAllPlatesDirty();
        }
    private void AddCoinEffect(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject coin = CoinsPool.Instance.GetCoinItem();
            if (coin != null)
            {
                coin.transform.SetParent(this.transform);
                
                coin.transform.position = this.transform.position + new Vector3(0, 0.3f, 0);

                coin.transform.localScale = Vector3.zero;

                // 
                Vector2 randomOffset = Random.insideUnitCircle * 0.6f; // 0.6f dağılma genişliği 
                Vector3 targetPos = coin.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

                // 
                Tween.Position(coin.transform, endValue: targetPos, duration: 0.4f, ease: Ease.OutBack);
                
                Tween.Scale(coin.transform, endValue: Vector3.one * 0.5f, duration: 0.4f, ease: Ease.OutBack);

                activeCoins.Add(coin);
            }
        }
    }
        public void AddMoneyToTable(List<OrderItemSO> orderItems)
        {
            float totalAmount = 0f;
            foreach (var item in orderItems)
            {
                totalAmount += item.foodPrice;
            }
            currentMoneyOnTable += totalAmount;
        }
    
        public bool HasDirtyPlates()
        {
            // Mantık: En az bir kirli tabak var mı?
            for (int i = 0; i < seatPoints.Length; i++)
            {
                if (visuals.IsPlateDirty(i)) return true;
            }
            return false;
        }

        public void CleanAllDirtyPlates()
        {
            bool anythingCleaned = false;
            int dirtyPlateCount = 0;

            for (int i = 0; i < seatPoints.Length; i++)
            {
                // Kirli mi kontrolünü Visuals'a sor
                if (visuals.IsPlateDirty(i)) dirtyPlateCount++;

                if (visuals.IsPlateDirty(i))
                {
                    visuals.HidePlate(i); // Görseli gizle
                    anythingCleaned = true;
                }
            }

            visuals.PlayCoinEffect(activeCoins);

            DishesStation.Instance.AddDirtyDishes(dirtyPlateCount);
            if (anythingCleaned)
            {
                EventBus.Publish(new PlayerEvents.OnMoneyCollectedEvent(currentMoneyOnTable));
                currentMoneyOnTable = 0f;
            }
        }

        // --- EVENT HANDLERS ---
        
        private void HandleCustomerOrderDecision(CustomerEvents.CustomerMakeDecisionEvent data)// siparişe karar verdikten sonra 
        {
            if (IsCustomerAtThisTable(data.Customer))// artık bu kıssımda uyarı işartei çıkmalı
            {
                //visuals.ShowOrderBubble(data.Customer.currentOrderItems);
                //visuals.ShowReadyToOrder(data.Customer.GetPatienceDuration());
            }
        }

        private void HandleFoodServed(CustomerEvents.FoodServedEvent data) // yemek verildğiğinde
        {
            if (IsCustomerAtThisTable(data.Customer))
            {
                visuals.HideAllUI();
            }
        }

        private void HandleCustomerLeft(CustomerEvents.CustomerLeftEvent data)
        {
            if(IsCustomerAtThisTable(data.Customer)) 
            {
                visuals.HideAllUI();
            }
        }

        private bool IsCustomerAtThisTable(CustomerController customer)
        {
            if (customer == null) return false; 
            return customer.currentTable == this;
        }

        // --- UI ve GÖRSEL YÖNLENDİRMELERİ ---

        public void ShowReadyToOrderVisuals(float patienceDuration)
        {
            visuals.ShowReadyToOrder(patienceDuration);
        }

        public void ShowWaitingForFoodVisuals(float foodPrepDuration , List<OrderItemSO> orderItems ) // yemeği bekleme görseli
        {
            visuals.ShowWaitingForFood(orderItems, foodPrepDuration);
        }

        public void ClearTableVisuals()
        {
            visuals.HideAllUI();
        }
            
        public void SelectTableEffect()
        {
            visuals.PlayInteractionPunch();
        }

        private void HandleCustomerSelectedVisuals(CustomerEvents.CustomerSelectedEvent customer)
        {
            for( int i = 0; i < seatedCustomersGroup.Length; i++)
            {
                if(seatedCustomersGroup[i] != null ) return; 
            }

            if(customer.Customer.customers.Count > seatPoints.Length) 
            {
                return;
            }
            else
            {
                visuals.PlaySelectionEffect(true);
            }
        }

        private void HandleCustomerDeselectedVisuals(CustomerEvents.CustomerDeselectedEvent customer)
        {
            visuals.PlaySelectionEffect(false);
        }
    }