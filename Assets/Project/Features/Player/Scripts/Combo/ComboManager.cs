
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 3f;
    [SerializeField] private int maxComboCount = 10;

    public int CurrentComboCount { get; private set; } = 0;
    public float CurrentComboTimeRatio => (comboResetTime > 0) ? _currentTimer / comboResetTime : 0;

    public float _currentTimer = 0f;
    private CancellationTokenSource _cts;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        EventBus.Subscribe<PlayerEvents.ComboTriggeredEvent>(OnComboTriggered);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlayerEvents.ComboTriggeredEvent>(OnComboTriggered);
        CancelTimer();
    }

    private void OnComboTriggered(PlayerEvents.ComboTriggeredEvent evt)
    {
        AddCombo(evt._worldPosition);
    }

    private void AddCombo(Vector3 position)
    {
        CancelTimer();

        CurrentComboCount++;

        if (CurrentComboCount > maxComboCount)
        {
            CurrentComboCount = maxComboCount;
        }

        if(CurrentComboCount > 1)
        {
            EventBus.Publish(new UIEvent.ComboUpdateEvent(CurrentComboCount , position));
            
        }

        _cts = new CancellationTokenSource();
        ComboCountdownRoutine(_cts.Token).Forget();
    }

    private async UniTaskVoid ComboCountdownRoutine(CancellationToken token)
    {
        _currentTimer = comboResetTime;

        while (_currentTimer > 0)
        {
            bool isCancelled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token).SuppressCancellationThrow();
            
            if (isCancelled) return; // İptal edildiyse fonksiyondan çık

            _currentTimer -= Time.deltaTime;

        }

        ResetCombo();
    }

    private void ResetCombo()
    {
        CurrentComboCount = 0;
        _currentTimer = 0;
        
        //EventBus.Publish(new UIEvent.ComboUpdateEvent(CurrentComboCount));
    }

    private void CancelTimer()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}