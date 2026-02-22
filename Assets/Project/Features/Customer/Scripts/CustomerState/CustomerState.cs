using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomerState
{

    //customer controller reference
    protected CustomerController customerController;
    public CustomerState(CustomerController customerController)
    {
        this.customerController = customerController;
    }

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();


}
