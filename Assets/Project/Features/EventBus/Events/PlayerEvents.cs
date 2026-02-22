using UnityEngine;

public static class PlayerEvents 
{

    public struct ComboTriggeredEvent
    {
        public Vector3 _worldPosition;
        public ComboTriggeredEvent( Vector3 worldPosition)
        {
            _worldPosition = worldPosition;
        }
    }


    
    public struct OrderReadyEvent
    {
        public CustomerController Owner;
        public OrderItemSO OrderData;

        public OrderReadyEvent(CustomerController owner, OrderItemSO data)
        {
            Owner = owner;
            OrderData = data;
        }
    }


    public struct OnMoneyCollectedEvent
    {
        public float amount;

        public OnMoneyCollectedEvent( float amt)
        {
            amount = amt;
        }
    }


}
