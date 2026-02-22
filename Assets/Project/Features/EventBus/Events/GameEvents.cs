using UnityEngine;

public static class GameEvents 
{
    public struct RushTimeEvent
    {
        public bool isRushHour;

        public RushTimeEvent(bool isRushHour)
        {
            this.isRushHour = isRushHour;
        }
    }

    public struct OnErrorEvent
    {
        
    }
}
