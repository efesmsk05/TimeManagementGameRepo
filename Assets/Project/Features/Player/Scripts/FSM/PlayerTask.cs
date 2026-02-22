using System;
using UnityEngine;

[System.Serializable]
public class PlayerTask
{
    public Vector3 targetPosition;
    public Action onArrivalAction;

    public PlayerTask(Vector3 target, Action action)
    {
        this.targetPosition = target;
        this.onArrivalAction = action;
    }
}