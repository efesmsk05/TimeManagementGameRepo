using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputManager inputActions;


    public static event Action<Vector2> OnClickStartedEvent;
    public static event Action<Vector2> OnClickCanceledEvent;

    public static event Action<Vector2> OnClickPerofmedEvent;
    private void Awake()
    {
        inputActions = new PlayerInputManager();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.clickCounter.started += OnClickStarted;
        inputActions.Player.clickCounter.canceled += OnClickCanceled;
        inputActions.Player.clilckPos.performed += OnClickPerformed;
        
    }



    private void OnDisable()
    {
        // Artık doğru şekilde abonelikten çıkabiliriz
        inputActions.Player.clickCounter.started -= OnClickStarted;
        inputActions.Player.clickCounter.canceled -= OnClickCanceled;
        inputActions.Player.clilckPos.performed -= OnClickPerformed;

        inputActions.Disable();
    }

    private void OnClickStarted(InputAction.CallbackContext ctx)
    {

        Vector2 mousePos = inputActions.Player.clilckPos.ReadValue<Vector2>();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        OnClickStartedEvent?.Invoke(worldPos);
    }

    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        
        Vector2 releasePos = inputActions.Player.clilckPos.ReadValue<Vector2>();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(releasePos);
        worldPos.z = 0;

        OnClickCanceledEvent?.Invoke(worldPos);
    }

        private void OnClickPerformed(InputAction.CallbackContext context)
    {
        

        Vector2 performPos = inputActions.Player.clilckPos.ReadValue<Vector2>();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(performPos);
        worldPos.z = 0;

        OnClickPerofmedEvent?.Invoke(worldPos);
    }
}