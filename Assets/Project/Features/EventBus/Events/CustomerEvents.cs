using UnityEngine;

public static class CustomerEvents 
{
    public struct CustomerSelectedEvent
    {
        public CustomerController Customer;
        public CustomerSelectedEvent(CustomerController customer) { Customer = customer; }
    }

    public struct CustomerDeselectedEvent
    {
        public CustomerController Customer;
        public CustomerDeselectedEvent(CustomerController customer) { Customer = customer; }
    }


    public struct CustomerMakeDecisionEvent{ // müşteri sipariş vermek üzere oturduğunda
            public CustomerController Customer;

            public TableController currentTable;
            public CustomerMakeDecisionEvent(CustomerController customer, TableController table) { Customer = customer; currentTable = table; }
    }

    public struct OrderTakenEvent // sipariş player tarafından alındığında
    {
        public CustomerController Customer;
        

    public OrderTakenEvent(CustomerController customer)
    {
        Customer = customer;
    }

    } 

    public struct FoodServedEvent // müşteri sipariş verildiğinde
    {
        public CustomerController Customer;

        // food kaldırıldı 
        public FoodServedEvent(CustomerController customer )
        {
            Customer = customer;
        }
    }


    public struct CustomerStartEatingEvent // müşteri yemeğe başladığında
    {
        public CustomerController Customer;

        public CustomerStartEatingEvent(CustomerController customer )
        {
            Customer = customer;
        }


    }




    //LEAVE

    public struct CustomerLeftEvent // müşteri mekandan ayrıldığında
    {
        public CustomerController Customer;

        public CustomerLeftEvent(CustomerController customer )
        {
            Customer = customer;
        }    

    }

    // dishes

}
