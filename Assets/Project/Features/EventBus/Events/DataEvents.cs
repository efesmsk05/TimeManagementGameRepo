
using UnityEngine;

public static class DataEvents 
{

    public struct OnUpgradeSuccessEvent
    {
        public UpgradeType upgradeType;

        public OnUpgradeSuccessEvent(UpgradeType upgradeType)
        {
            this.upgradeType = upgradeType;
        }
    }

    public struct OnReputationGainedEvent
    {
        public float amount;

        public OnReputationGainedEvent(float amount )
        {
            this.amount = amount;
        }

    }

}
