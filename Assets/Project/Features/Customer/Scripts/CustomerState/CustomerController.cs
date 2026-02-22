    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CustomerController : MonoBehaviour
    {

        public CustomerState currentState;

        [Header ("Components")]
        public CustomerMovement customerMovement;

        public List<Customer> customers; // artık gurup şeklide ilerleyece

        public List<Animator> animators;

        public Customer customer => (customers != null && customers.Count > 0) ? customers[0] : null;

        public Animator mainAnimator => (animators != null && animators.Count > 0) ? animators[0] : null;

        public Vector3 currentTargetPos;

        [Header(" Group Settings")]
        public float groupTotalPatience;

        public Transform[] groupWaitingPositions; // ilk start pozisyonları (müşteriler dükkandan çıktığında ve tekrar düknna girdiğinde buraya ışınlanacaklar)



        [Header ("Order Items")]
        public List<OrderItemSO> orderItems = new List<OrderItemSO>();
        public List<OrderItemSO> currentOrderItems = new List<OrderItemSO>(); // müşteri guruplarını total siparişi

        [Header("Patience Bar UI")]
        public CustomerPatienceUI patienceBarUI;
        //Customer Memory
        public TableController currentTable;

        
        


        public void Initialize(Vector3 targetPos)
        {
            foreach (var customer in customers)
            {
                customer.gameObject.SetActive(true);
                customer.Initialize();
            }

            SelectRandomOrder();

            // SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // if (spriteRenderer != null && orderDisplay != null)
            // {
            //     spriteRenderer.color = Color.white; // Varsayılan renk
                
            // }

            currentTargetPos = targetPos;

            ChangeState(new EnteringShopState(this));// state sıfırlıyoruzs


        }   

        

        
        public void DeSpawnSelf() // Müşteriyi havuza geri gönder ve verilerini sıfırla
        {
            currentOrderItems.Clear();
            currentTable = null;
           // currentSeatIndex = -1;
           foreach (var customer in customers)
            {
                customer.seatIndex = -1; // customer scriptindeki seat indexini sıfırlıyoruz

                
            }

            for (int i = 0; i < groupWaitingPositions.Length; i++)
            {
                if (i < customers.Count)
                {
                    customers[i].transform.localPosition = groupWaitingPositions[i].localPosition; // müşterileri start pozisyonlarına ışınlıyoruz
                }
            }

            OnCollider();
            CustomerPool.Instance.ReturnCustomer(this.gameObject);
        }


        void Awake()
        {
            customers = new List<Customer>(GetComponentsInChildren<Customer>());
        
            animators = new List<Animator>(GetComponentsInChildren<Animator>());

            customerMovement = GetComponent<CustomerMovement>();
        }
        void Start()
        {


        }

        void Update()
        {
            if(currentState != null)
            {
                currentState.Update();
            }
        }

        

        public void ChangeState(CustomerState newState)
        {
            if(currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;

            if(currentState != null)
            {
                currentState.Enter();
            }
        }



        // Functions
        public void SelectRandomOrder()
        {
            if (orderItems.Count <= 0) return;

            foreach (var customer in customers)
            {
                int randomIndex = Random.Range(0, orderItems.Count);
                currentOrderItems.Add(orderItems[randomIndex]);
                
            }


        }


        public void SetTotalPatience()
        {
            // müşteri gurubundaki en uzun süre

            float maxPrepTime = 0f;

            foreach (var item in currentOrderItems)
            {
                if (item.prepTime > maxPrepTime)
                {
                    maxPrepTime = item.prepTime;
                }
            }

            groupTotalPatience = maxPrepTime * customer.patienceMultiplier + 5f; 



        }

        public void AssingnSeat(TableController table)
        {
            currentTable = table; // guruba masa ataması 

            foreach (var customer in customers)// gurubun altındaki her bir customera seat indexi ataması yapıyoruz
            {
                for (int i = 0; i < table.seatPoints.Length; i++)
                {
                    if (!table.slotOccupied[i])
                    {
                        customer.seatIndex = i; // customer scriptindeki seat indexini atıyoruz
                        table.slotOccupied[i] = true; 
                        break;
                    }
                }
            }


        }

    public void UpdateTargetPosition(Vector3 newPos)
    {
        currentTargetPos = newPos;

        // NavMesh'e "Yürü" emrini ver
        // Agent bunu aldığı an fiziksel olarak yürümeye başlar.
        if (customerMovement != null && customerMovement.agent != null)
        {
            customerMovement.StartMoving();

            customerMovement.agent.SetDestination(newPos);
        }
    }

        public void OffCollider()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        public void OnCollider()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }


        // Patience Bar UI Güncelleme

        public void ShowPatienceBar(PatienceBarType type, float duration = 0f)
        {

            if (patienceBarUI != null)
            {
                patienceBarUI.SetupBar(duration , type);
            }
        }
    
        public void HidePatienceBar()
        {
            if (patienceBarUI != null)
            {
                patienceBarUI.HideBar();
            }
        }

    }

