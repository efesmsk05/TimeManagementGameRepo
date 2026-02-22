    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class WaitingForSeatState : TimedCustomerState   
    {
        private int queuePositionIndex;
        private int extraWaitTimePerCustomer = 5; 
        private float totalWaitTime;

        private float baseWaitTime =20f; // Temel bekleme süresi   

        public WaitingForSeatState(CustomerController controller) : base(controller) 
        {
        }

        public override float GetTotalTime()
        {
            int index = CustomerSpawner.Instance.waitingCustomers.IndexOf(customerController);
            if(index < 0)
            {
                index = 0; // Eğer müşteri listede yoksa, varsayılan olarak ilk sırada kabul et
            }

            return (baseWaitTime + (index * extraWaitTimePerCustomer)) * customerController.customer.patienceMultiplier;
    
        }

        protected override PatienceBarType GetPatienceBarType()
        {
            return PatienceBarType.standart; // hangi bar çalışıcak
        }

        public override void Enter()
        {
            customerController.mainAnimator.SetBool("isIdle", true); 

            base.Enter();

        }

        public override void Update()
        {
            // Müşteri seçilmeyi bekliyor...
            // Kuyruk ilerlerse UpdateTargetPosition çalışır, NavMesh yürütür ama State değişmez.
            // İstersen burada yürüme animasyonu kontrolü yapabilirsin:
            if (customerController.customerMovement.agent.velocity.sqrMagnitude > 0.1f)
                customerController.mainAnimator.SetBool("isIdle", false);
            else
                customerController.mainAnimator.SetBool("isIdle", true);
        }

        public override void Exit()
        {

            customerController.mainAnimator.SetBool("isIdle", false);

            base.Exit();

        }


        protected override void OnTimeout()
        {
            
            EventBus.Publish(new CustomerEvents.CustomerLeftEvent(customerController));
            CustomerSpawner.Instance.OnCustomerLeft(customerController);

            customerController.ChangeState(new LeavingShopState(customerController));
        }
    }