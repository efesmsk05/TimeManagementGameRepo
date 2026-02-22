    using System.Threading;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections.Generic;
using Unity.VisualScripting;


public class ClockManager : MonoBehaviour
    {
        public static ClockManager Instance;

        private CancellationTokenSource cts;

        [Header("Settings")]
        public int levelStartHour = 8;
        public int levelEndHour = 20;
        public float realSecondsPerHour = 2f;

        [Header ("Rush Settings")]
        // güne özel rush saatleri olaibilir bu ayarlar day data so dan yapılamlı 
        public int rushStartHour = 10;
        public int rushEndHour = 14; 

        public bool isRushHourStarted { get; private set; } = false;


        public void Initialize(int startHour, int endHour, float secondsPerHour )
        {
            Debug.Log(" Saat Yöneticisi Başlatıldı.");
            levelStartHour = startHour;
            levelEndHour = endHour;
            realSecondsPerHour = secondsPerHour;
        }


        void Awake()
        {

            Instance = this;
        }

        public void StartClock()
        {
            if(cts != null)
            {
                cts.Cancel(); 
                cts.Dispose(); 
            }
            cts = new CancellationTokenSource();
            DayClock(cts.Token).Forget();
        }

        private async UniTaskVoid DayClock(CancellationToken token)
        {
            Debug.Log(" Mesai Başladı");

            for (int currentHour = levelStartHour; currentHour < levelEndHour; currentHour++)
            {
                EventBus.Publish(new UIEvent.UpdateClockEvent(currentHour, 0)); 

                // Rush kontrolü

                if (!isRushHourStarted && currentHour >= rushStartHour && currentHour < rushEndHour)
                {
                    isRushHourStarted = true;
                    Debug.Log("Rush Saati Başladı!");
                    EventBus.Publish(new GameEvents.RushTimeEvent(true));
                }
                else if (isRushHourStarted && currentHour >= rushEndHour)
                {
                    isRushHourStarted = false;
                    Debug.Log(" Rush Saati Bitti!");
                    EventBus.Publish(new GameEvents.RushTimeEvent(false));
                }
                
                bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(realSecondsPerHour), cancellationToken: token).SuppressCancellationThrow();
                if (isCancelled) return;
            }

            EventBus.Publish(new UIEvent.UpdateClockEvent(levelEndHour, 0));
            Debug.Log("⏰ Mesai Bitti!");

            LevelManager.Instance.ChangeState(LevelState.Closing);
        }

        public void StopClock()
        {
            if(cts != null)
            {
                cts.Cancel(); 
                cts.Dispose(); 
                cts = null;    
                Debug.Log("⏰ Saat Durduruldu.");
            }
        }

        private void OnDestroy()
        {
            StopClock();
        }


    }
