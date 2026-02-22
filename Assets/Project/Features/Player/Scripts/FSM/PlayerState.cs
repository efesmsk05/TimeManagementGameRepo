using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState 
{
    protected PlayerController playerController;

    public PlayerState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();

    public abstract void OnClick(Vector2 pos);


}
