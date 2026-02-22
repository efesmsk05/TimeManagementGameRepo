using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingDecisionState : CustomerState
{
    private float decisionTime = 2f; // karar verme süresi
    private float timer = 0f;

    private bool decisionMade = false;
    public MakingDecisionState(CustomerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        // Müşteri karar verme animasyonunu başlat
       
    }

    public override void Update()
    {
        if(!decisionMade)
        {
            timer += Time.deltaTime;
            
            if(timer >= decisionTime)
            {
                decisionMade = true;
                //customerController.SelectRandomOrder();

                // Müşteri artık siparişini bekleme durumuna geçer
                customerController.ChangeState(new WaitingForWaiterState(customerController ));
            }
        }
    }

    public override void Exit()
    {

    }
}
